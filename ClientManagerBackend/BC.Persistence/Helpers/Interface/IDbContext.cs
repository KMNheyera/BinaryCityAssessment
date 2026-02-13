using System.Data;
using static Dapper.SqlMapper;

namespace BC.Persistence.Helpers.Interface
{
    public interface IDbContext : IAsyncDisposable
    {
        Task<int> ExecuteAsync(string sqlOrProc, object? param = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.StoredProcedure, CancellationToken ct = default);

        // Executes and returns a scalar typed value.
        Task<T?> ExecuteScalarAsync<T>(string sqlOrProc, object? param = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.StoredProcedure, CancellationToken ct = default);

        // Query list of POCOs.
        Task<IEnumerable<T>> QueryListAsync<T>(string sqlOrProc, object? param = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.StoredProcedure, CancellationToken ct = default);

        // Query single or default POCO.
        Task<T?> QuerySingleOrDefaultAsync<T>(string sqlOrProc, object? param = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.StoredProcedure, CancellationToken ct = default);

        // QueryMultiple wrapper; user supplies a function to map the GridReader to result.
        Task<TResult> QueryMultipleAsync<TResult>(string sqlOrProc, object? param, Func<GridReader, Task<TResult>> mapAsync, IDbTransaction? transaction = null, CommandType commandType = CommandType.StoredProcedure, CancellationToken ct = default);

    }
}