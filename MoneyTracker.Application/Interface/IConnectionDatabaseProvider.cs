using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Application.Interface
{
    public interface IConnectionDatabaseProvider
    {
        string GetConnectionString();

        void Set(string host, string port, string database, string username, string password);
    }
}
