using Microsoft.EntityFrameworkCore;
using MoneyTracker.Domain.Entity;
using MoneyTracker.Domain.Enums;

namespace MoneyTracker.Infrastructure.Persistence
{
    internal class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {

        }

        public DbSet<AccountEntity> Accounts => Set<AccountEntity>();
        public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
        public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Setup EF untuk table Accounts
            modelBuilder.Entity<AccountEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AccountName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Amount).HasDefaultValue(0);
                entity.HasIndex(e => e.AccountName).IsUnique();
                entity.HasIndex(e => e.AccountNumber).IsUnique();
            });

            //Setup EF untuk table Categories
            modelBuilder.Entity<CategoryEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedByUser).ValueGeneratedOnUpdate().HasDefaultValueSql("CURRENT_USER");
                entity.Property(e => e.UpdatedDate).ValueGeneratedOnUpdate().HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.CategoryName).IsUnique();
            });

            //Setup EF untuk Table Transactions
            modelBuilder.Entity<TransactionEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.CreateByUser).IsRequired();
                entity.Property(e => e.TransactionDate).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasConversion(
                    v => v == TransactionStatus.Reconciled ? "R" : "U",
                    v => v == "R" ? TransactionStatus.Reconciled : TransactionStatus.Unreconciled)
                .HasDefaultValue(TransactionStatus.Unreconciled).HasMaxLength(1);
                
                entity.Property(e => e.Kredit).HasDefaultValue(0);
                entity.Property(e => e.Debit).HasDefaultValue(0);
                entity.Property(e => e.LastBalance).IsRequired();

                entity.HasOne(e => e.Account)
                      .WithMany(a=> a.Transactions)
                      .HasForeignKey(e => e.AccountId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                      .WithMany(c=>c.Transactions)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
