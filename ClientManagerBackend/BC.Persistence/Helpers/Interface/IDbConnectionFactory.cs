using System.Data;

namespace BC.Persistence.Helpers.Interface
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
