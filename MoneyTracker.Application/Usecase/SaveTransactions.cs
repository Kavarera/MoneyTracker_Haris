
using MoneyTracker.Application.DTO;
using MoneyTracker.Domain.Entity;
using MoneyTracker.Domain.Enums;
using MoneyTracker.Domain.Interface;

namespace MoneyTracker.Application.Usecase
{
    public class SaveTransactions
    {
        private readonly ITransactionsRepository repo;

        public SaveTransactions(ITransactionsRepository repo)
        {
            this.repo = repo;
        }

        public async Task ExecuteAsync(IEnumerable<TransactionDTO> dtos)
        {
                var transactions = dtos.Select(dto =>
                    new TransactionEntity {
                        Id = dto.Id,
                        TransactionDate = dto.TransactionDate,
                        Note = dto.Description,
                        Status = dto.Status == "R" ? TransactionStatus.Reconciled : TransactionStatus.Unreconciled,
                        CategoryId = dto.CategoryId,
                        Kredit = dto.Kredit,
                        Debit = dto.Debit
                    }).ToList();
                await repo.UpdateRangeAsync(transactions);
        }
    }
}
