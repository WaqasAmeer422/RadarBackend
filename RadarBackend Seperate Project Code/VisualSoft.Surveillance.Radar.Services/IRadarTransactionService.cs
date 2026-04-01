using VisualSoft.Surveillance.Radar.Domain;
using VisualSoft.Surveillance.Radar.Domain.Models;

namespace VisualSoft.Surveillance.Radar.Services
{
    public interface IRadarTransactionService
    {


        Task<IEnumerable<RadarTransactionDto>?> GetAllTransactions();
        Task<(IEnumerable<RadarTransactionDto>? result, int totalCount)> GetAllTransactionsPagedItems(PaginationParameters paginationParams);
        Task<RadarTransactionDto?> GetTransactionById(Guid id);
        Task<IEnumerable<RadarTransactionDto>?> GetTransactionByCriteria(TransactionFilterRequestDto? criteria);

        //Task<(IEnumerable<RadarTransactionDto>? result, int totalCount)> GetTransactionByCriteriaPagedItems(TransactionFilterRequestDto model, PaginationParameters paginationParameters);
        //Task<RadarTransactionDto?> NavigateTransactionByCriteria(NavigateTransactionFilterDto? criteria);

    }
}