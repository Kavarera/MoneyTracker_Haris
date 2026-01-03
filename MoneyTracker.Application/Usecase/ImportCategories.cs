using MoneyTracker.Domain.Interface;

namespace MoneyTracker.Application.Usecase
{
    public class ImportCategories
    {
        private readonly IReadCsvCategories _readCsvCategories;

        public ImportCategories(IReadCsvCategories readCsvCategories)
        {
            _readCsvCategories = readCsvCategories;
        }

        public async Task ExecuteAsync(string path, CancellationToken cancellationToken = default)
        {
            await _readCsvCategories.ReadCategories(path);
        }
    }
}
