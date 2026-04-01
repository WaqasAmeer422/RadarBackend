using System.Data;

namespace VisualSoft.Surveillance.Radar.Data.Infrastructure
{
    public interface IQueryExecuter
    {
        Task<T> QueryFirstAsync<T>(IDbConnection connection, string query, object parameter);
        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string query, object parameter);
        IEnumerable<T> Query<T>(IDbConnection connection, string query, object parameter);
        int Execute(IDbConnection connection, string query, object parameter = null);
        T QuerySingle<T>(IDbConnection connection, string query, object parameter, IDbTransaction transaction = null);
        T QueryFirstOrDefault<T>(IDbConnection connection, string query, object parameter);
    }
}
