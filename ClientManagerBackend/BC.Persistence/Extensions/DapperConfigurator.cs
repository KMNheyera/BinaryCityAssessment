using BC.Persistence.Helpers.Implementation;
using BC.Persistence.Helpers.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BC.Persistence.Extensions
{
    public static class DapperConfigurator
    {

        private static bool _isConfigured = false;
        private static readonly object _lock = new();

        /// <summary>
        /// Configure Serilog globally and wire it to Microsoft ILogger
        /// </summary>
        public static IServiceCollection AddDbContextService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            lock (_lock)
            {
                if (_isConfigured)
                {
                    return services; // Already configured, skip
                }

                var connectionString = configuration.GetSection("DbConnection").Get<string>() ?? throw new ArgumentNullException("DbConnectionString");

                services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

                services.AddSingleton<IDbContext, DbContext>();

                return services;
            }
        }
    }
}
