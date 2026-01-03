using MoneyTracker.Application.DTO;
using MoneyTracker.Domain.Interface;
using System.Linq;

namespace MoneyTracker.Application.Usecase
{
    public  class GetTransactions
    {
        private readonly ITransactionsRepository repository;

        public GetTransactions(ITransactionsRepository transactionRepository)
        {
            repository= transactionRepository;
        }

        public async Task<IReadOnlyList<TransactionDTO>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        {
            var transactions = await repository.GetAllAsync(cancellationToken);

            // di sini tempat aturan aplikasi
            // contoh filtering / sorting dll
            return transactions
                .OrderBy(c => c.CreatedDate)
                .Select(c => new TransactionDTO(c.Id, c.TransactionDate, c.Note, c.Kredit,c.Debit,c.LastBalance,c.Category.CategoryName, 
                c.CategoryId,c.Status == Domain.Enums.TransactionStatus.Reconciled ? "R" : "U", c.Account.AccountName, c.Account.Id)).OrderByDescending(c=>c.TransactionDate)
                .ToList();
        }
    }
}
