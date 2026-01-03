

using MoneyTracker.Domain.Enums;

namespace MoneyTracker.Domain.Entity
{
    public class TransactionEntity
    {
        

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; }
        public string CreateByUser { get; set; }
        public string UpdatedByUser { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionStatus Status { get; set; } = TransactionStatus.Unreconciled;
        public string Note { get; set; } = string.Empty;
        public decimal Kredit { get; set; } = 0;
        public decimal Debit { get; set; } = 0;
        public decimal LastBalance { get; set; } = 0;

        // FK — EXPLICIT
        public int AccountId { get; set; }
        public int CategoryId { get; set; }

        public AccountEntity Account { get; set; }
        public CategoryEntity Category { get; set; }
    }
}
