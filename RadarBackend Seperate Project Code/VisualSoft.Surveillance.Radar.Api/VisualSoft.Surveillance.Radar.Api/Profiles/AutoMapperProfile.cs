using AutoMapper;
using VisualSoft.Surveillance.Radar.Api.Models;
using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Api.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BaseDto, BaseResponse>();

            //  Radar Data Mapping
            CreateMap<RadarEventModel, RadarTransactionDto>();
            CreateMap<RadarTransactionDto, TransactionRadarResponse>();
            CreateMap<TransactionFilterRequest, TransactionFilterRequestDto>();
            CreateMap<PaginationParametersRequest, PaginationParameters>();
            CreateMap<NavigateTransactionFilterRequest, NavigateTransactionFilterDto>();
        }
    }
}
