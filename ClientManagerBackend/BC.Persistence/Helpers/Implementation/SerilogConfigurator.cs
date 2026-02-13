using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace BC.Persistence.Helpers.Implementation
{
    public static class SerilogConfigurator
    {
        /// <summary>
        /// Configure Serilog globally and wire it to Microsoft ILogger
        /// </summary>
        public static IServiceCollection AddSerilogWithFileLogging(
            this IServiceCollection services,
            IConfiguration configuration,
            string applicationName = "GenericSym")
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            // Configure Serilog globally
            ConfigureSerilog(configuration, applicationName);

            // Register Serilog with Microsoft ILogger
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });

            return services;
        }

        /// <summary>
        /// Configure Serilog with file logging
        /// </summary>
        private static void ConfigureSerilog(IConfiguration configuration, string applicationName = "GenericSym")
        {
            var logFilePath = GetLogFilePath(configuration, applicationName);
            var minimumLevel = GetMinimumLogLevel(configuration);
            var appName = applicationName ?? configuration["Application:Name"] ?? "Application";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(minimumLevel)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", appName)
                .Enrich.WithProperty("Environment",
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
                .WriteTo.File(
                    path: logFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: configuration.GetValue<int>("Logging:RetainedFileCountLimit", 30),
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: configuration.GetValue<long>("Logging:FileSizeLimitBytes", 10485760),
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Application} {Message:lj}{NewLine}{Exception}",
                    shared: true)
                .WriteTo.Console(
                    outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            // Log initialization
            Log.Information("Serilog initialized for {Application}", appName);
            Log.Information("Log file: {LogFilePath}", logFilePath);
        }

        private static string GetLogFilePath(IConfiguration configuration, string applicationName)
        {
            var basePath = configuration["Logging:FilePath"] ?? "Logs/app-.log";

            if (!string.IsNullOrEmpty(applicationName) && basePath.Contains("app"))
            {
                basePath = basePath.Replace("app", applicationName.ToLower());
            }

            // Ensure log directory exists
            var directory = System.IO.Path.GetDirectoryName(basePath);
            if (!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            return basePath;
        }

        private static void CheckString(string value, string name)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"Configuration value for '{name}' is missing or empty.");
            }

            if (value is ['a', 'e', 'i', 'o', 'u'])
            {
                Console.WriteLine("value ha a letter between a vowel");
            }
        }

        private static LogEventLevel GetMinimumLogLevel(IConfiguration configuration)
        {
            var level = configuration["Logging:LogLevel:Default"] ?? "Information";

            return level.ToLower() switch
            {
                "verbose" => LogEventLevel.Verbose,
                "debug" => LogEventLevel.Debug,
                "information" => LogEventLevel.Information,
                "warning" => LogEventLevel.Warning,
                "error" => LogEventLevel.Error,
                "fatal" or "critical" => LogEventLevel.Fatal,
                _ => LogEventLevel.Information
            };
        }
    }
}
