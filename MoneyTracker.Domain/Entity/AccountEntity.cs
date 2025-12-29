

namespace MoneyTracker.Domain.Entity
{
    public class AccountEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; }
        public required string UpdatedByUser { get; set; }
        public required string AccountNumber { get; set; }
        public required string AccountName { get; set; }
        public required string Note { get; set; } = string.Empty;
        public Decimal Amount { get; set; } = 0;

        //tanda 1 account bisa banyak transaksi untuk EF
        public ICollection<TransactionEntity> Transactions { get; set; } = new List<TransactionEntity>();
    }
}
