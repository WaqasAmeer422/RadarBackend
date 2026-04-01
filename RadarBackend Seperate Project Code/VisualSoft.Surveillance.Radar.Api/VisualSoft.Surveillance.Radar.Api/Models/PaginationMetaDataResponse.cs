namespace VisualSoft.Surveillance.Radar.Api.Models
{
    public class PaginationMetaDataResponse
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
