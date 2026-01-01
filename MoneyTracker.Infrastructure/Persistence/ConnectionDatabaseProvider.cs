using MoneyTracker.Application.Interface;

namespace MoneyTracker.Infrastructure.Persistence
{
    internal class ConnectionDatabaseProvider : IConnectionDatabaseProvider
    {
        private string _connectionString;

        public ConnectionDatabaseProvider(string host, int port, string database, string username, string password)
        {
            _connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};";
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }

        public void Set(string host, string port, string database, string username, string password)
        {
            _connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};";
        }
    }
}
