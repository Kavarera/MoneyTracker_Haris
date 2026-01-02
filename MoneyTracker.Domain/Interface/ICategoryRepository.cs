using MoneyTracker.Domain.Entity;

namespace MoneyTracker.Domain.Interface
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<CategoryEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
