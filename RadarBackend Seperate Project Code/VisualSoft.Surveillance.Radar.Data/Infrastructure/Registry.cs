using Microsoft.Extensions.DependencyInjection;

namespace VisualSoft.Surveillance.Radar.Data.Infrastructure
{
    public static class Registry
    {
        public static void AddServicesToContainer(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IConnectionFactory, ConnectionFactory>();
            services.AddScoped<IQueryExecuter, QueryExecuter>();
        }
    }
}
