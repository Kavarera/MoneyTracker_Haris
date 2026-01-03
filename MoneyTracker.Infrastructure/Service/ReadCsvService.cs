using Microsoft.Extensions.Logging;
using MoneyTracker.Domain.Entity;
using MoneyTracker.Domain.Interface;
using MoneyTracker.Infrastructure.Persistence;

namespace MoneyTracker.Infrastructure.Service
{
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
