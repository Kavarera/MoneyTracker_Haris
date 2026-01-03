
using MoneyTracker.Domain.Entity;

namespace MoneyTracker.Domain.Interface
{
    public interface IReadCsvCategories
    {
        Task ReadCategories(string path, CancellationToken ct = default);
    }
    public interface IReadCsvAccounts
    {
        Task ReadAccounts(string path, CancellationToken ct = default);
    }
    public interface IReadCsvTransactions
    {
        Task ReadTransactions(string path, CancellationToken ct = default);
    }
}
