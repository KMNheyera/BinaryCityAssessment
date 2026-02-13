namespace BC.Persistence.Settings
{
    public class ConsoleLoggingConfig
    {
        public bool Enabled { get; set; } = true;
        public string OutputTemplate { get; set; } =
            "[{Level:u3}] {Message:lj}{NewLine}{Exception}";
    }
}
