using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using VisualSoft.Surveillance.Radar.Data.Infrastructure;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Configurations;
using VisualSoft.Surveillance.Radar.Domain.Models;
using VisualSoft.Surveillance.Radar.Services.Adapters;
using VisualSoft.Surveillance.Radar.Services.RabbitMQ;

namespace VisualSoft.Surveillance.Radar.Services.Handlers
{
    public interface ITransactionLogServiceHandler
    {
        ITransactionBaseService GetService(string Transactiontype);
    }


    public class TransactionServiceHandler : ITransactionLogServiceHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserIdentificationModel _userIdentificationModel;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IOrganisationAdapter _organisationAdapter;
        private readonly IServiceConfiguration _serviceConfiguration;
        private readonly IRabbitMQEventService _rabbitMQEventService;

        public TransactionServiceHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserIdentificationModel userIdentificationModel,
            ILogger logger, IMemoryCache memoryCache, IOrganisationAdapter organisationAdapter, IServiceConfiguration serviceConfiguration
            , IRabbitMQEventService rabbitMQEventService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userIdentificationModel = userIdentificationModel;
            _logger = logger;
            _memoryCache = memoryCache;
            _organisationAdapter = organisationAdapter;
            _serviceConfiguration = serviceConfiguration;
            _rabbitMQEventService = rabbitMQEventService;
        }
        public ITransactionBaseService GetService(string Transactiontype)
        {
            switch (Transactiontype)
            {      
                // CASE for Human Detection:
                case Constants.DeviceEvent.HUMAN_RADAR:
                    return new RADARTransactionService(_unitOfWork, _mapper, _userIdentificationModel, _logger,
                        _memoryCache, _rabbitMQEventService, _organisationAdapter, _serviceConfiguration);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
