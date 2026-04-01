using AutoMapper;
using Serilog;
using VisualSoft.Surveillance.Radar.Data.Infrastructure;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Configurations;
using VisualSoft.Surveillance.Radar.Domain.Models;
using VisualSoft.Surveillance.Radar.Services;
using VisualSoft.Surveillance.Radar.Services.Adapters;

namespace VisualSoft.Surveillance.Radar.Services
{
    public class RadarTransactionService : IRadarTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        protected readonly IUserIdentificationModel _userIdentificationModel;
        protected readonly ILogger _logger;
        protected readonly IOrganisationAdapter _organisationAdapter;
        protected readonly IServiceConfiguration _serviceConfiguration;

        public RadarTransactionService(IUnitOfWork unitOfWork, IMapper mapper, IUserIdentificationModel userIdentificationModel,
            ILogger logger, IOrganisationAdapter organisationAdapter, IServiceConfiguration serviceConfiguration)
        {
            _unitOfWork = unitOfWork;
            _userIdentificationModel = userIdentificationModel;
            _mapper = mapper;
            _logger = logger;
            _organisationAdapter = organisationAdapter;
            _serviceConfiguration = serviceConfiguration;
        }

        public async Task<IEnumerable<RadarTransactionDto>?> GetAllTransactions()
        {
            var results = new List<RadarTransactionDto>();

            if (_userIdentificationModel.IsSuperUser)
            {
                results = (await _unitOfWork.RadarTransactionRepository.GetAllTransactions()).ToList();
            }
            else
            {
                results = (await _unitOfWork.RadarTransactionRepository.GetAllTransactions(_userIdentificationModel.OrganizationId)).ToList();
            }

            return await TransformTransactionsWithOrganisation(results);
        }

        public async Task<(IEnumerable<RadarTransactionDto>? result, int totalCount)> GetAllTransactionsPagedItems(PaginationParameters paginationParams)
        {
            var result = new List<RadarTransactionDto>();
            var totalCount = 0;
            if (_userIdentificationModel.IsSuperUser)
            {
                var (items, count) = await _unitOfWork.RadarTransactionRepository.GetAllTransactionsPagedItems(paginationParams);
                result = items.ToList();
                totalCount = count;
            }
            else
            {
                var (items, count) = await _unitOfWork.RadarTransactionRepository.GetAllTransactionsPagedItems(_userIdentificationModel.OrganizationId, paginationParams);
                result = items.ToList();
                totalCount = count;
            }
            return (await TransformTransactionsWithOrganisation(result), totalCount);
        }

        public async Task<RadarTransactionDto?> GetTransactionById(Guid id)
        {
            var result = await _unitOfWork.RadarTransactionRepository.GettransactionById(id);
            if (result == null)
            {
                return null;
            }
            return await TransformTransactionWithOrganisation(result);
        }

        public async Task<IEnumerable<RadarTransactionDto>?> GetTransactionByCriteria(TransactionFilterRequestDto? criteria)
        {
            var results = await _unitOfWork.RadarTransactionRepository.GetTransactionByCriteria(criteria);
            return await TransformTransactionsWithOrganisation(results);
        }

        //public async Task<(IEnumerable<RadarTransactionDto>? result, int totalCount)> GetTransactionByCriteriaPagedItems(TransactionFilterRequestDto model, PaginationParameters paginationParameters)
        //{
        //    var result = new List<RadarTransactionDto>();
        //    var totalCount = 0;
        //    if (!_userIdentificationModel.IsSuperUser)
        //    {
        //        model.OrganisationId = _userIdentificationModel.OrganizationId;
        //    }

        //    var (items, count) = await _unitOfWork.RadarTransactionRepository.GetTransactionByCriteriaPagedItems(model, paginationParameters);
        //    result = items.ToList();
        //    totalCount = count;
        //    return (await TransformTransactionsWithOrganisation(result), totalCount);
        //}


        //public async Task<RadarTransactionDto?> NavigateTransactionByCriteria(NavigateTransactionFilterDto? criteria)
        //{
        //    var result = await _unitOfWork.RadarTransactionRepository.NavigateTransactionByCriteria(criteria);
        //    if (result == null)
        //    {
        //        return null;
        //    }
        //    return await TransformTransactionWithOrganisation(result);
        //}
        ////end


        private async Task<IEnumerable<RadarTransactionDto>> TransformTransactionsWithOrganisation(IEnumerable<RadarTransactionDto?> transactions)
        {
            var organisations = await _organisationAdapter.GetOrganisations();

            var transactionsWithOrganizationNames = from transaction in transactions
                                                    join org in organisations.Organisations
                                                    on transaction.OrganisationId equals org.Id
                                                    select new RadarTransactionDto
                                                    {
                                                        Id = transaction.Id,
                                                        CreatedBy = transaction.CreatedBy,
                                                        CreatedDate = transaction.CreatedDate,
                                                        IsActive = transaction.IsActive,
                                                        OrganisationId = transaction.OrganisationId,
                                                        UpdatedBy = transaction.UpdatedBy,
                                                        UpdatedDate = transaction.UpdatedDate,
                                                        OrganisationName = org.Name,
                                                        DeviceId = transaction.DeviceId,
                                                        Location = transaction.Location,
                                                        Level1Location = Level1Location(transaction.Location),
                                                        Level2Location = Level2Location(transaction.Location),
                                                        // RADAR SPECIFIC FIELDS 
                                                        Presence = transaction.Presence,
                                                        PresenceType = transaction.PresenceType,
                                                        RadarSourceMac = transaction.RadarSourceMac,
                                                        AgrinodeMac = transaction.AgrinodeMac,
                                                        DeviceTimestamp = transaction.DeviceTimestamp,
                                                        TransactionEvent = transaction.TransactionEvent, 
                                                        
                                                    };
            return transactionsWithOrganizationNames.OrderBy(x => x.Location);
        }
        private string Level1Location(string location)
        {
            location = string.IsNullOrWhiteSpace(location) ? "N/A" : location;
            var locationArrays = location.Split(":");
            if (locationArrays.Length > 1)
            {
                return locationArrays[0].Trim();
            }
            return location;
        }

        private string Level2Location(string location)
        {
            var locationArrays = location.Split(":");
            if (locationArrays.Length > 1)
            {
                return locationArrays[1].Trim();
            }
            return location;
        }
        private async Task<RadarTransactionDto> TransformTransactionWithOrganisation(RadarTransactionDto transaction)
        {
            if (transaction.OrganisationId.HasValue)
            {
                var organisation = await _organisationAdapter.GetOrganisationById(transaction.OrganisationId.Value);
                transaction.OrganisationName = organisation.Name;
            }
            return transaction;
        }
    }
}