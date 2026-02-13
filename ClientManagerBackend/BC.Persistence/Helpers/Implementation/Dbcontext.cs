using BC.Persistence.Helpers.Interface;
using Dapper;
using System.Data;
using static Dapper.SqlMapper;

namespace BC.Persistence.Helpers.Implementation
{
    public class DbContext : IDbContext
    {
        private readonly IDbConnectionFactory _factory;
        private bool _disposed;

        public DbContext(IDbConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        private async Task<T> UseConnectionAsync<T>(Func<IDbConnection, Task<T>> work, CancellationToken ct)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(DbContext));

            // Create and open a new connection for each operation.
            // This leverages ADO.NET connection pooling and is thread-safe.
            await using var conn = (System.Data.Common.DbConnection)_factory.CreateConnection();
            await conn.OpenAsync(ct).ConfigureAwait(false);
            return await work(conn).ConfigureAwait(false);
        }

        public Task<int> ExecuteAsync(string sqlOrProc, object? param = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.StoredProcedure, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(sqlOrProc)) throw new ArgumentNullException(nameof(sqlOrProc));
            return UseConnectionAsync(conn => conn.ExecuteAsync(new CommandDefinition(sqlOrProc, param, transaction, commandType: commandType, cancellationToken: ct)), ct);
        }

        public Task<T?> ExecuteScalarAsync<T>(string sqlOrProc, object? param = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.StoredProcedure, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(sqlOrProc)) throw new ArgumentNullException(nameof(sqlOrProc));
            return UseConnectionAsync(conn => conn.ExecuteScalarAsync<T?>(new CommandDefinition(sqlOrProc, param, transaction, commandType: commandType, cancellationToken: ct)), ct);
        }

        public Task<IEnumerable<T>> QueryListAsync<T>(string sqlOrProc, object? param = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.StoredProcedure, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(sqlOrProc)) throw new ArgumentNullException(nameof(sqlOrProc));
            return UseConnectionAsync(conn => conn.QueryAsync<T>(new CommandDefinition(sqlOrProc, param, transaction, commandType: commandType, cancellationToken: ct)), ct);
        }

        public Task<T?> QuerySingleOrDefaultAsync<T>(string sqlOrProc, object? param = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.StoredProcedure, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(sqlOrProc)) throw new ArgumentNullException(nameof(sqlOrProc));
            return UseConnectionAsync(conn => conn.QuerySingleOrDefaultAsync<T>(new CommandDefinition(sqlOrProc, param, transaction, commandType: commandType, cancellationToken: ct)), ct);
        }

        public Task<TResult> QueryMultipleAsync<TResult>(string sqlOrProc, object? param, Func<GridReader, Task<TResult>> mapAsync, IDbTransaction? transaction = null, CommandType commandType = CommandType.StoredProcedure, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(sqlOrProc)) throw new ArgumentNullException(nameof(sqlOrProc));
            if (mapAsync == null) throw new ArgumentNullException(nameof(mapAsync));

            return UseConnectionAsync(async conn =>
            {
                using var multi = await conn.QueryMultipleAsync(new CommandDefinition(sqlOrProc, param, transaction, commandType: commandType, cancellationToken: ct)).ConfigureAwait(false);
                return await mapAsync(multi).ConfigureAwait(false);
            }, ct);
        }

        public ValueTask DisposeAsync()
        {
            _disposed = true;
            return ValueTask.CompletedTask;
        }
    }
}

