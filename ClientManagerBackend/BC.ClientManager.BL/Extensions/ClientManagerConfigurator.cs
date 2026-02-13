using BC.ClientManager.BL.Repository.Impementation;
using BC.ClientManager.BL.Repository.Interface;
using BC.ClientManager.BL.Service.Implementation;
using BC.ClientManager.BL.Service.Interface;
using BC.Persistence.Extensions;
using BC.Persistence.Helpers.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BC.ClientManager.BL.Extensions
{
    public static class ClientManagerConfigurator
    {
        private static bool _isConfigured = false;
        private static readonly object _lock = new();

        /// <summary>
        /// Configure Serilog globally and wire it to Microsoft ILogger
        /// </summary>
        public static IServiceCollection AddClientManagerService(
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

                services.AddSerilogWithFileLogging(configuration);
                services.AddDbContextService(configuration);

                services.AddScoped<IContactRepository, ContactRepository>();
                services.AddScoped<IClientRepository, ClientRepository>();

                services.AddScoped<IClientService, ClientService>();
                services.AddScoped<IContactService, ContactService>();

                return services;
            }
        }
    }
}
