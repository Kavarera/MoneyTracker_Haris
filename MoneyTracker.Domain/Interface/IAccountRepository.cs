using MoneyTracker.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Domain.Interface
{
    public interface IAccountRepository
    {
        Task<IReadOnlyList<AccountEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
