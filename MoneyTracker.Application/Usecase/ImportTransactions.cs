using MoneyTracker.Domain.Interface;

namespace MoneyTracker.Application.Usecase
{
    public class ImportTransactions
    {
        private readonly IReadCsvTransactions _readCsvTransactions;
        public ImportTransactions(IReadCsvTransactions readCsvTransactions)
        {
            _readCsvTransactions = readCsvTransactions;
        }
        public async Task ExecuteAsync(string path, CancellationToken cancellationToken = default)
        {
            await _readCsvTransactions.ReadTransactions(path, cancellationToken);
        }
    }
}
