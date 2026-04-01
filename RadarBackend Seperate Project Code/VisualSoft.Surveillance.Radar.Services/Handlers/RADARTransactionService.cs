using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using OneOf;
using OneOf.Types;
using Serilog;
using VisualSoft.Surveillance.Radar.Data.Infrastructure;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Configurations;
using VisualSoft.Surveillance.Radar.Domain.Models;
using VisualSoft.Surveillance.Radar.Domain.Utils;
using VisualSoft.Surveillance.Radar.Services.Adapters;
using VisualSoft.Surveillance.Radar.Services.Models;
using VisualSoft.Surveillance.Radar.Services.RabbitMQ;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Dapper.SqlMapper;
using static VisualSoft.Surveillance.Radar.Domain.Constants.LookUps;
namespace VisualSoft.Surveillance.Radar.Services.Handlers
{
    public class RADARTransactionService : TransactionBaseService
    {
        private const string DEVICE_TIMESTAMP_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private readonly IMemoryCache _memoryCache;
        private readonly IRabbitMQEventService _rabbitMQEventService;

        public RADARTransactionService(IUnitOfWork unitOfWork, IMapper mapper, IUserIdentificationModel userIdentificationModel,
            ILogger logger, IMemoryCache memoryCache, IRabbitMQEventService rabbitMQEventService,
            IOrganisationAdapter organisationAdapter, IServiceConfiguration serviceConfiguration)
            : base(unitOfWork, mapper, userIdentificationModel, logger, organisationAdapter, serviceConfiguration)
        {
            _memoryCache = memoryCache;
            _rabbitMQEventService = rabbitMQEventService;
        }

        public override async Task<OneOf<IGenertedData, ValidationResult>> ProcessData(object model)
        {
            try
            {
                RadarEventModel radarEventModel = (RadarEventModel)model;
                if (radarEventModel == null)
                {
                    return ValidationResult.Error("radarEventModel", Constants.Errors.ERROR_ANPR_MODEL_IS_NULL);
                }
                if (string.IsNullOrWhiteSpace(radarEventModel.RadarSourceMac))
                {
                    return ValidationResult.Error("radarEventModel", $"{Constants.Errors.ERROR_MAC_ADDRESS_IS_INVALID} MacAddress: Null");
                }

                // 1. Device Lookup by MAC
                var device = await _unitOfWork.DeviceRepository.GetByMacAddress(radarEventModel.RadarSourceMac);

                if (device == null)
                {
                    return ValidationResult.Error("radarEventModel", $"{Constants.Errors.ERROR_MAC_ADDRESS_IS_INVALID} MacAddress: {radarEventModel.RadarSourceMac}");
                }

                // 2. Prepare Data for Database (Mapping to Radar Table fields)
                // HumanDetectionLog replace with RadarTransactionLog
                RadarTransactionDto transaction = await PrepareDataForDataBase(device, radarEventModel);

                // 3. Persist Data (Full Transaction Logic)
                var radarTransactionData = await PersistData(transaction);

                // 4. RabbitMQ Alarm logic
                if (transaction.Presence)
                {
                    _logger.Information("Radar Presence Detected at {Location}", transaction.Location);
                    _rabbitMQEventService?.RaiseAlaramEvent(Constants.AlaramRules.RADAR_DETECTION, device.Id, "PRESENCE_DETECTED", ExtensionMethods.SerializeToUtf8Bytes(radarEventModel));
                }

                return radarTransactionData;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, Constants.Errors.ERROR_EXCEPTION_OCCURED);
                return ValidationResult.Error("Exception", ex.StackTrace);
            }
        }

        private async Task<RadarTransactionDto> PrepareDataForDataBase(DeviceDetailDto? device, RadarEventModel radarEventModel)
        {
            DateTime deviceTimeStamp = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(radarEventModel.DeviceTimestamp))
            {
                DateTime.TryParse(radarEventModel.DeviceTimestamp, out deviceTimeStamp);
            }
            // Radar DateTime Parser
            
            //DateTime deviceTimestamp;
            //if (DateTime.TryParseExact(data.deviceTimestamp, DEVICE_TIMESTAMP_FORMAT,
            //    System.Globalization.CultureInfo.InvariantCulture,
            //    System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            //{
            //    deviceTimestamp = parsedDate;
            //}
            //else
            //{
            //    _logger.Warning("Invalid Radar Timestamp format: {Time}. Expected: {Format}", data.deviceTimestamp, DEVICE_TIMESTAMP_FORMAT);
            //    deviceTimestamp = DateTime.Now;
            //}

            
            var transaction = new RadarTransactionDto()
            {
                RadarSourceMac = radarEventModel.RadarSourceMac,
                AgrinodeMac = radarEventModel.AgrinodeMac,
                DeviceId = device.Id,
                OrganisationId = device.OrganisationId,
                TransactionEvent = TransactionSource.RADAR,
                Presence = radarEventModel.Presence,
                PresenceType = radarEventModel.PresenceType,
                Location = await buildTransactionLocation(device.Id),
                DeviceTimestamp = deviceTimeStamp.Equals(DateTime.MinValue) ? null : deviceTimeStamp,
                CreatedBy = Constants.UPDATED_BY_DEFAULT_VALUE,
                UpdatedBy = Constants.UPDATED_BY_DEFAULT_VALUE
            };
            return transaction;
        }

        private async Task<RadarGeneratedData> PersistData(RadarTransactionDto transaction)
        {
            _unitOfWork.BeginTransaction();
            try
            {
                _logger.Debug(Constants.Informations.INFO_CREATING_TRANSACTION, new { Mac = transaction.RadarSourceMac });

                transaction = await _unitOfWork.RadarTransactionRepository.CreateTransaction(transaction);

                if (transaction == null)
                {
                    throw new Exception(Constants.Errors.ERROR_NO_RECORD_INSERTED.Replace("{@Variables}", "RadarLog"));
                }

                _unitOfWork.Commit();
                return new RadarGeneratedData 
                {
                    Transaction = transaction,

                };
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        // --- Caching and Location Logic (Kept as requested) ---

        private async Task<List<LookupDto>?> GetAllLookupsFromCache()
        {
            List<LookupDto>? lookups = null;
            var lookupsCacheKey = $"{DateTime.Now:ddMMyyyy}_Lookups";

            if (_memoryCache.TryGetValue(lookupsCacheKey, out lookups))
            {
                return lookups;
            }

            lookups = (await _unitOfWork.LookupRepository.GetAllLookups()).ToList();

            if (lookups.Count > 0)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
                _memoryCache.Set(lookupsCacheKey, lookups, cacheEntryOptions);
                return lookups;
            }
            return null;
        }

        private async Task<string> buildTransactionLocation(Guid deviceId)
        {
            string resultedLocation = string.Empty;
            var deviceSetting = await _unitOfWork.DeviceSettingRepository.GetDeviceSettingsByDeviceId(deviceId);
            if (deviceSetting != null && deviceSetting.Any())
            {
                var locations = deviceSetting.Where(x => x.SettingName.Equals(DeviceLocation.LOOKUP_NAME));
                if (locations.Any())
                {
                    var lookups = await GetAllLookupsFromCache();
                    if (lookups == null || lookups.Count == 0) return string.Empty;

                    var locationsLookups = lookups.Where(x => x.Name.Equals(DeviceLocation.LOOKUP_NAME));
                    foreach (var location in locations)
                    {
                        var keylocation = locationsLookups.FirstOrDefault(x => x.Code.Equals(location.SettingValue));
                        if (keylocation != null)
                        {
                            resultedLocation = resultedLocation == string.Empty ? keylocation.Description : resultedLocation + "," + keylocation.Description;
                        }
                    }
                }
            }
            return resultedLocation;
        }
    }
}