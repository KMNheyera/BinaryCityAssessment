using BC.Persistence.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.Reflection;
namespace BC.Persistence.Extensions
{
    public static class SerilogConfigurator
    {
        private static bool _isConfigured = false;
        private static readonly object _lock = new();

        /// <summary>
        /// Configure Serilog globally and wire it to Microsoft ILogger
        /// </summary>
        public static IServiceCollection AddSerilogWithFileLogging(
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

                try
                {
                    // 1. Configure Serilog first (before adding to DI)
                    ConfigureSerilog(configuration);

                    // 2. Register FileLoggingConfig as singleton
                    var loggingConfig = configuration.GetSection("Logging:File").Get<FileLoggingConfig>() ?? new FileLoggingConfig();
                    services.AddSingleton(loggingConfig);

                    // 3. Register Serilog with Microsoft ILogger
                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders(); // Clear default providers
                        loggingBuilder.AddSerilog(); // Add Serilog
                    });

                    _isConfigured = true;
                }
                catch (Exception ex)
                {
                    // Fallback to console logging
                    Console.WriteLine($"Failed to configure Serilog: {ex.Message}");
                    Console.WriteLine("Falling back to console logging...");

                    // Ensure at least basic logging is available
                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.AddConsole();
                        loggingBuilder.AddDebug();
                    });
                }
            }

            return services;
        }

        /// <summary>
        /// Configure Serilog with file logging
        /// </summary>
        private static void ConfigureSerilog(IConfiguration configuration)
        {
            // Get FileLoggingConfig from configuration
            var loggingConfig = configuration.GetSection("Logging:File").Get<FileLoggingConfig>() ?? new FileLoggingConfig();

            // Get application settings from configuration
            var appConfigSection = configuration.GetSection("Application");
            var appName = appConfigSection["Name"] ?? Assembly.GetEntryAssembly()?.GetName().Name ?? "Application";
            var appVersion = appConfigSection["Version"] ?? "1.0.0";

            // Get log file path, replacing app name placeholder
            var logFilePath = loggingConfig.FilePath;
            if (logFilePath.Contains("app") && !string.IsNullOrEmpty(appName))
            {
                logFilePath = logFilePath.Replace("app", appName.ToLower());
            }

            // Ensure log directory exists
            var directory = Path.GetDirectoryName(logFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Get log levels from configuration
            var logLevelSection = configuration.GetSection("Logging:LogLevel");
            var defaultLogLevel = logLevelSection["Default"] ?? loggingConfig.DefaultLogLevel;
            var microsoftLogLevel = logLevelSection["Microsoft"] ?? loggingConfig.MicrosoftLogLevel;
            var systemLogLevel = logLevelSection["System"] ?? loggingConfig.SystemLogLevel;

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(ParseLogLevel(defaultLogLevel))
                .MinimumLevel.Override("Microsoft", ParseLogLevel(microsoftLogLevel))
                .MinimumLevel.Override("System", ParseLogLevel(systemLogLevel))
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", appName)
                .Enrich.WithProperty("Version", appVersion)
                .Enrich.WithProperty("Environment",
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
                .WriteTo.File(
                    path: logFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: loggingConfig.RetainedFileCountLimit,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: loggingConfig.FileSizeLimitBytes,
                    outputTemplate: loggingConfig.OutputTemplate,
                    shared: true)
                .WriteTo.Console(outputTemplate: loggingConfig.ConsoleOutputTemplate)
                .CreateLogger();

            // Log initialization
            Log.Information("Serilog initialized for {Application} v{Version}", appName, appVersion);
            Log.Information("Log file: {LogFilePath}", logFilePath);
        }

        private static LogEventLevel ParseLogLevel(string level)
        {
            return level?.ToLower() switch
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

        /// <summary>
        /// Check if Serilog has already been configured
        /// </summary>
        public static bool IsConfigured => _isConfigured;
    }
}
