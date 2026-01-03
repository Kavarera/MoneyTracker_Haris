using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyTracker.Domain.Entity;
using MoneyTracker.Domain.Interface;
using MoneyTracker.Infrastructure.Persistence;

namespace MoneyTracker.Infrastructure.Service
{
    internal class ReadCsvTransactionsService : IReadCsvTransactions
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ReadCsvTransactionsService> _logger;

        public ReadCsvTransactionsService(AppDbContext db, ILogger<ReadCsvTransactionsService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task ReadTransactions(string path, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    throw new FileNotFoundException("CSV file not found.", path);

                var lines = await File.ReadAllLinesAsync(path, ct);
                if (lines.Length < 2) return;

                // 1. Ambil data Account (objek utuh) dan Category (ID saja) ke memory
                // Kita ambil objek Account utuh agar bisa update saldo secara massal
                var accounts = await _db.Accounts.ToDictionaryAsync(a => a.AccountName, ct);
                var categoryMap = await _db.Categories.ToDictionaryAsync(c => c.CategoryName, c => c.Id, ct);

                var transactions = new List<TransactionEntity>();

                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var fields = line.Split(';');
                    if (fields.Length < 6) continue;

                    try
                    {
                        string accountName = fields[1].Trim();
                        string categoryName = fields[2].Trim();

                        // Validasi
                        if (!accounts.TryGetValue(accountName, out var account) ||
                            !categoryMap.TryGetValue(categoryName, out int catId))
                        {
                            _logger.LogWarning($"Baris {i + 1} skip: Account/Category '{accountName}'/'{categoryName}' tidak ada.");
                            continue;
                        }

                        // Parse nilai numerik
                        decimal kredit = decimal.Parse(fields[4].Trim().Replace(",", ""));
                        decimal debit = decimal.Parse(fields[5].Trim().Replace(",", ""));

                        var transaction = new TransactionEntity
                        {
                            TransactionDate = DateTime.SpecifyKind(DateTime.Parse(fields[0].Trim()), DateTimeKind.Utc),
                            AccountId = account.Id,
                            CategoryId = catId,
                            Note = fields[3].Trim(),
                            Kredit = kredit,
                            Debit = debit,
                            Status = Domain.Enums.TransactionStatus.Unreconciled,
                            // Kita simpan balance setelah transaksi (optional, tapi bagus untuk audit)
                            LastBalance = account.Amount + kredit - debit
                        };

                        // 2. Update saldo di objek account yang ada di memory
                        account.Amount += kredit;
                        account.Amount -= debit;

                        transactions.Add(transaction);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Gagal parsing baris {i + 1}: {ex.Message}");
                    }
                }

                if (transactions.Any())
                {
                    // 3. Simpan perubahan transaksi
                    await _db.Transactions.AddRangeAsync(transactions, ct);

                    // EF Core otomatis mendeteksi perubahan 'Amount' pada objek 'account' 
                    // karena objek tersebut diambil dari _db.Accounts tadi.
                    await _db.SaveChangesAsync(ct);

                    _logger.LogInformation($"{transactions.Count} transaksi berhasil diimpor dan saldo diupdate.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fatal saat import transaksi.");
                throw;
            }
        }
    }

    internal class ReadCsvAccountsService : IReadCsvAccounts
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ReadCsvAccountsService> _logger;

        public ReadCsvAccountsService(AppDbContext db, ILogger<ReadCsvAccountsService> log)
        {
            _db = db;
            _logger = log;
        }

        public async Task ReadAccounts(string path, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    throw new FileNotFoundException("CSV file not found.", path);
                }

                var accs = new List<AccountEntity>();
                var lines = await File.ReadAllLinesAsync(path);

                if (lines.Length < 2)
                {
                    return; // Return empty list if no data
                }

                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Gunakan Split dengan opsi Trim agar lebih bersih
                    var fields = line.Split(';');

                    // VALIDASI KRUSIAL: 
                    // Jika baris cuma "Transport;", fields.Length hanya 2 (Indeks 0 dan 1)
                    // Akses fields[2] akan langsung CRASH.
                    if (fields.Length < 2) continue;

                    try
                    {
                        var acc = new AccountEntity
                        {
                            // Ambil Nama (Indeks 0)
                            AccountName = fields[0].Trim(),

                            // Ambil Note (Indeks 1) - Cek dulu apakah indeks 1 ada atau tidak
                            // Jika ada isinya pake fields[1], jika tidak ada pake string kosong
                            Note = fields.Length > 1 ? fields[1].Trim() : string.Empty,
                        };

                        accs.Add(acc);
                        
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"Gagal di baris {i + 1}: {ex.Message}");
                    }
                }
                await _db.Accounts.AddRangeAsync(accs, ct);
                await _db.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to read categories from CSV: {ex.Message}");
                throw;
            }
        }
    }


    internal class ReadCsvCategoriesService : IReadCsvCategories
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ReadCsvCategoriesService> _logger;

        public ReadCsvCategoriesService(AppDbContext db, ILogger<ReadCsvCategoriesService> log)
        {
            _db = db;
            _logger = log;
        }

        public async Task ReadCategories(string path, CancellationToken ct = default)
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    throw new FileNotFoundException("CSV file not found.", path);
                }

                var categories = new List<CategoryEntity>();
                var lines = await File.ReadAllLinesAsync(path);

                if (lines.Length < 2)
                {
                    return; // Return empty list if no data
                }

                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Gunakan Split dengan opsi Trim agar lebih bersih
                    var fields = line.Split(';');

                    // VALIDASI KRUSIAL: 
                    // Jika baris cuma "Transport;", fields.Length hanya 2 (Indeks 0 dan 1)
                    // Akses fields[2] akan langsung CRASH.
                    if (fields.Length < 2) continue;

                    try
                    {
                        var category = new CategoryEntity
                        {
                            // Ambil Nama (Indeks 0)
                            CategoryName = fields[0].Trim(),

                            // Ambil Note (Indeks 1) - Cek dulu apakah indeks 1 ada atau tidak
                            // Jika ada isinya pake fields[1], jika tidak ada pake string kosong
                            Note = fields.Length > 1 ? fields[1].Trim() : string.Empty,
                        };

                        categories.Add(category);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"Gagal di baris {i + 1}: {ex.Message}");
                    }
                }
                await _db.Categories.AddRangeAsync(categories,ct);
                await _db.SaveChangesAsync(ct);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,$"Failed to read categories from CSV: {ex.Message}");
                throw;
            }
        }
    }
}
