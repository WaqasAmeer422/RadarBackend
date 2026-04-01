namespace VisualSoft.Surveillance.Radar.Api.Models
{
    public class PaginationParametersRequest
    {
        private const int MaxPageSize = 500; // Set a maximum page size to prevent abuse
        /// <summary>
        /// Page number to fecth
        /// </summary>
        public int PageNumber { get; set; } = 1; // Default to the first page

        private int _pageSize = 50; // Default page size

        /// <summary>
        /// Page Size to Fetch
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
