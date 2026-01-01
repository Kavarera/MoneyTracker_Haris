using MoneyTracker.Application.Interface;
using Npgsql;

namespace MoneyTracker.Infrastructure.Service
{
    public class ConnectionDbTest : IConnectionDbTest
    {
        public async Task<(bool Success, string Message)> TestConnectionAsync(
            string host, int port, string database, string username, string password, CancellationToken token = default)
        {
            var cs = $"Host={host};Port={port};Database={database};Username={username};Password={password};Timeout=5;CommandTimeout=5";

            try
            {
                await using var conn = new NpgsqlConnection(cs);
                await conn.OpenAsync(token);
                return (true, "Connection successful.");
            }
            catch (OperationCanceledException)
            {
                return (false, "Connection test cancelled.");
            }
            catch (PostgresException ex)
            {
                return (false, $"PostgreSQL error: {ex.MessageText}");
            }
            catch (NpgsqlException ex)
            {
                return (false, $"Connection error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Unexpected error: {ex.Message}");
            }
        }
    }
}
