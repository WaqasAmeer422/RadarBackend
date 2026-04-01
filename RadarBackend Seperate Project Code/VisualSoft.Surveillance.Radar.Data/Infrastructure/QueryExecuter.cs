using Dapper;
using System.Data;

namespace VisualSoft.Surveillance.Radar.Data.Infrastructure
{
    public class QueryExecuter : IQueryExecuter
    {
        public async Task<T> QueryFirstAsync<T>(IDbConnection connection, string query, object parameter)
        {
            return await connection.QueryFirstAsync<T>(query, parameter);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string query, object parameter)
        {
            return await connection.QueryAsync<T>(query, parameter);
        }

        public IEnumerable<T> Query<T>(IDbConnection connection, string query, object parameter)
        {
            return connection.Query<T>(query, parameter, null, true);
        }

        public int Execute(IDbConnection connection, string query, object parameter = null)
        {
            return connection.Execute(query, parameter, null);
        }

        public T QuerySingle<T>(IDbConnection connection, string query, object parameter, IDbTransaction transaction = null)
        {
            return connection.QuerySingle<T>(query, parameter, transaction);
        }

        public T QueryFirstOrDefault<T>(IDbConnection connection, string query, object parameter)
        {
            return connection.QueryFirstOrDefault<T>(query, parameter, null);
        }
    }
}
