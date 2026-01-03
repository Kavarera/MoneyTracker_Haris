
using MoneyTracker.Domain.Entity;

namespace MoneyTracker.Domain.Interface
{
    public interface IReadCsvCategories
    {
        Task ReadCategories(string path);
    }
    public interface IReadCsvAccounts
    {
        List<AccountEntity> ReadAccounts();
    }
    public interface IReadCsvTransactions
    {
        List<TransactionEntity> ReadTransactions();
    }
}
