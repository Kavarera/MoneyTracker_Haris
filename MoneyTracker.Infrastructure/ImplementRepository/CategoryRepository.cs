using Microsoft.EntityFrameworkCore;
using MoneyTracker.Domain.Entity;
using MoneyTracker.Domain.Interface;
using MoneyTracker.Infrastructure.Persistence;


namespace MoneyTracker.Infrastructure.ImplementRepository
{
    internal class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _db;

        public CategoryRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IReadOnlyList<CategoryEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Categories.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
