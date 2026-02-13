using BC.Persistence.Helpers.Implementation;
using BC.Persistence.Helpers.Interface;

namespace BC.ClientManager.Test.Configurations
{
    public static class PersistanceConfig
    {

        public static IDbConnectionFactory ResolveDBConnection()
        {
            var connectionString = "Server=localhost;Database=ClientManagerDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;";
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("DB_CONNECTION_STRING environment variable is not set.");
            }
            return new SqlConnectionFactory(connectionString);
        }
    }
}
