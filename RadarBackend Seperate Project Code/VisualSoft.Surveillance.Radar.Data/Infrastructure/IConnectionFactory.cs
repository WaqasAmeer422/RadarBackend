using Npgsql;

namespace VisualSoft.Surveillance.Radar.Data.Infrastructure
{
    public interface IConnectionFactory
    {
        string ConnectionString { get; }
        NpgsqlConnection Connection { get; }
    }
}
