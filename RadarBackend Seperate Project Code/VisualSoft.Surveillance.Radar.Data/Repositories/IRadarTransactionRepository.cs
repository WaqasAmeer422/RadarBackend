using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Models;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Data.Repositories
{
    public interface IRadarTransactionRepository
    {
        Task<RadarTransactionDto?> CreateTransaction(RadarTransactionDto model);
        Task<RadarTransactionDto?> Update(RadarTransactionDto model);
        Task<IEnumerable<RadarTransactionDto>> GetAllTransactions();
        Task<IEnumerable<RadarTransactionDto>?> GetAllTransactions(Guid organisationId);
        Task<(IEnumerable<RadarTransactionDto>?, int totalCount)> GetAllTransactionsPagedItems(PaginationParameters paginationParams);
        Task<(IEnumerable<RadarTransactionDto>?, int totalCount)> GetAllTransactionsPagedItems(Guid organisation_id, PaginationParameters paginationParams);
       
        Task<RadarTransactionDto?> GettransactionById(Guid id);
        Task<IEnumerable<RadarTransactionDto>?> GetTransactionByCriteria(TransactionFilterRequestDto? criteria);
        
        //Task<IEnumerable<RadarTransactionDto>?> GetMatchingTransactions(DateTime fromDate, Guid organisationId);
        //Task<(IEnumerable<RadarTransactionDto>?, int totalCount)> GetTransactionByCriteriaPagedItems(TransactionFilterRequestDto model, PaginationParameters paginationParams);
        //Task<RadarTransactionDto?> NavigateTransactionByCriteria(NavigateTransactionFilterDto? criteria);
    }
}