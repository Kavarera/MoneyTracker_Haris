

namespace MoneyTracker.Domain.Entity
{
    public class CategoryEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; }
        public required string UpdatedByUser { get; set; }
        public required string CategoryName { get; set; }
        public required string Note { get; set; } = string.Empty;

        // Tanda 1 kategori bisa banyak transaksi untuk EF (Entity Framework/ ORM Framework)
        public ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
    }
}
