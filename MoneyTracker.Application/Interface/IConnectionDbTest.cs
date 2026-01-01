using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Application.Interface
{
    public interface IConnectionDbTest
    {
        Task<bool> TestConnectionAsync(string host, int port, string database, string username, string password, CancellationToken token = default);
    }
}
