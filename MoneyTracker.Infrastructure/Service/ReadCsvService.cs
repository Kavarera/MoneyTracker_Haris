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

                // 1. Ambil semua Account dan Category ke memory agar tidak query berulang kali di dalam loop (Performa)
                var accountMap = await _db.Accounts.ToDictionaryAsync(a => a.AccountName, a => a.Id, ct);
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
                        // Kolom CSV: DateTransaction[0]; Account[1]; Category[2]; Note[3]; Kredit[4]; Debit[5]
                        string accountName = fields[1].Trim();
                        string categoryName = fields[2].Trim();

                        // Validasi apakah Account & Category ada di DB
                        if (!accountMap.TryGetValue(accountName, out int accId) ||
                            !categoryMap.TryGetValue(categoryName, out int catId))
                        {
                            _logger.LogWarning($"Baris {i + 1} dilewati: Account '{accountName}' atau Category '{categoryName}' tidak ditemukan di Database.");
                            continue;
                        }

                        var transaction = new TransactionEntity
                        {
                            TransactionDate = DateTime.Parse(fields[0].Trim()), // Format: 1 Jan 2025
                            AccountId = accId,
                            CategoryId = catId,
                            Note = fields[3].Trim(),
                            // Parse angka dengan format ribuan Indonesia (koma)
                            Kredit = decimal.Parse(fields[4].Trim().Replace(",", "")),
                            Debit = decimal.Parse(fields[5].Trim().Replace(",", "")),

                            // Default values
                            Status = Domain.Enums.TransactionStatus.Unreconciled,
                            CreateByUser = "System_CSV_Import",
                            UpdatedByUser = "System_CSV_Import",
                        };

                        transactions.Add(transaction);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Gagal parsing baris {i + 1}: {ex.Message}");
                    }
                }

                if (transactions.Any())
                {
                    await _db.Transactions.AddRangeAsync(transactions, ct);
                    await _db.SaveChangesAsync(ct);
                    _logger.LogInformation($"{transactions.Count} transaksi berhasil diimpor.");
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
