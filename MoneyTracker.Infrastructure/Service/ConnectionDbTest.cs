using MoneyTracker.Application.Interface;

namespace MoneyTracker.Infrastructure.Service
{
    public class ConnectionDbTest : IConnectionDbTest
    {
        public async Task<bool> TestConnectionAsync(string host, int port, string database, string username, string password, CancellationToken token = default)
        {
             return await Task.FromResult(true);
        }
    }
}
