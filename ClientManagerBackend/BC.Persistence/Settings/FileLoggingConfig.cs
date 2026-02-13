

namespace BC.Persistence.Settings
{
    public class FileLoggingConfig
    {
        public string FilePath { get; set; } = "Logs/app-.log";
        public int RetainedFileCountLimit { get; set; } = 30;
        public long FileSizeLimitBytes { get; set; } = 10485760; // 10MB
        public string OutputTemplate { get; set; } = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Application} {Message:lj}{NewLine}{Exception}";
        public string ConsoleOutputTemplate { get; set; } = "[{Level:u3}] {Message:lj}{NewLine}{Exception}";
        public bool EnableConsole { get; set; } = true;

        // Log levels
        public string DefaultLogLevel { get; set; } = "Information";
        public string MicrosoftLogLevel { get; set; } = "Warning";
        public string SystemLogLevel { get; set; } = "Warning";
    }
}
