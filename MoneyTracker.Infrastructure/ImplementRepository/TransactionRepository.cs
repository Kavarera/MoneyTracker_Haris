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

        public async Task UpdateRangeAsync(IEnumerable<TransactionEntity> datas)
        {
            foreach (var dto in datas)
            {
                var entity = await _db.Transactions.FindAsync(dto.Id);

                if (entity == null)
                    continue;

                // hitung selisih antara value baru dan lama
                var kreditDiff = dto.Kredit - entity.Kredit;
                var debitDiff = dto.Debit - entity.Debit;

                //update saldo
                entity.LastBalance += kreditDiff;
                entity.LastBalance -= debitDiff;


                entity.TransactionDate = dto.TransactionDate;
                entity.Note = dto.Note;
                entity.Status = dto.Status;

                entity.CategoryId = dto.CategoryId;
                entity.Kredit = dto.Kredit;
                entity.Debit = dto.Debit;

                // update value transaksi
                entity.Kredit = dto.Kredit;
                entity.Debit = dto.Debit;
            }

            await _db.SaveChangesAsync();
        }
    }
}
