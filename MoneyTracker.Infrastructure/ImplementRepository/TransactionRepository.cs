using Microsoft.EntityFrameworkCore;
using MoneyTracker.Domain.Entity;
using MoneyTracker.Domain.Interface;
using MoneyTracker.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Infrastructure.ImplementRepository
{
    internal class TransactionRepository : ITransactionsRepository
    {
        private readonly AppDbContext _db;
        public TransactionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<TransactionEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Transactions.Include(t=>t.Category).Include(t=>t.Account).AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
