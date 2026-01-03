using MoneyTracker.Domain.Entity;
using MoneyTracker.Domain.Interface;
using MoneyTracker.Infrastructure.Persistence;

namespace MoneyTracker.Infrastructure.Service
{
    public class ReadCsvCategoriesService : IReadCsvCategories
    {
        private readonly AppDbContext _db;

        public ReadCsvCategoriesService(AppDbContext db)
        {
            _db = db;
        }

        public async Task ReadCategories(string path)
        {
            if(string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                throw new FileNotFoundException("CSV file not found.", path);
            }

            var categories = new List<CategoryEntity>();
            var lines = await File.ReadAllLinesAsync(path);

            if(lines.Length <= 1)
            {
                return; // Return empty list if no data
            }

            for(int i=1; i < lines.Length; i++)
            {
                var line = lines[i];
                var fields = line.Split(';');
                if (fields.Length == 0)
                {
                    continue; // Skip invalid lines
                }
                try
                {
                    var category = new CategoryEntity
                    {
                        CategoryName = fields[1],
                        Note = fields[2],
                    };
                    categories.Add(category);
                }
                catch (Exception ex)
                {
                    // Log or handle parsing errors
                    Console.WriteLine($"Error parsing line {i + 1}: {ex.Message}");
                }
            }
            await _db.Categories.AddRangeAsync(categories);
        }
    }
}
