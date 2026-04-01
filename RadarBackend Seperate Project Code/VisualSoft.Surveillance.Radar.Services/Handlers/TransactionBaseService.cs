using AutoMapper;
using OneOf;
using Serilog;
using VisualSoft.Surveillance.Radar.Data.Infrastructure;
using VisualSoft.Surveillance.Radar.Domain.Configurations;
using VisualSoft.Surveillance.Radar.Domain.Models;
using VisualSoft.Surveillance.Radar.Domain.Utils;
using VisualSoft.Surveillance.Radar.Services.Adapters;
using VisualSoft.Surveillance.Radar.Services.Models;

namespace VisualSoft.Surveillance.Radar.Services.Handlers
{
    public abstract class TransactionBaseService : ITransactionBaseService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IUserIdentificationModel _userIdentificationModel;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;
        protected readonly IOrganisationAdapter _organisationAdapter;
        protected readonly IServiceConfiguration _serviceConfiguration;


        public TransactionBaseService(IUnitOfWork unitOfWork, IMapper mapper, IUserIdentificationModel userIdentificationModel,
            ILogger logger, IOrganisationAdapter organisationAdapter, IServiceConfiguration serviceConfiguration)
        {
            _unitOfWork = unitOfWork;
            _userIdentificationModel = userIdentificationModel;
            _mapper = mapper;
            _logger = logger;
            _organisationAdapter = organisationAdapter;
            _serviceConfiguration = serviceConfiguration;
        }

        public abstract Task<OneOf<IGenertedData, ValidationResult>> ProcessData(object model);
        //public abstract Task<OneOf<object, ValidationResult>> ProcessData(object model);
    }
}
