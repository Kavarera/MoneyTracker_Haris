
using Microsoft.EntityFrameworkCore;
using MoneyTracker.Domain.Entity;
using MoneyTracker.Domain.Interface;
using MoneyTracker.Infrastructure.Persistence;

namespace MoneyTracker.Infrastructure.ImplementRepository
{
    internal class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _dbContext;
        
        public AccountRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<AccountEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Accounts.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
