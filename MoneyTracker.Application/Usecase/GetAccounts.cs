using MoneyTracker.Application.DTO;
using MoneyTracker.Domain.Interface;
namespace MoneyTracker.Application.Usecase
{

    public class GetAccounts
    {
        private readonly IAccountRepository _accountRepository;

        public GetAccounts(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IReadOnlyList<AccountDTO>> ExecuteAsync(
        CancellationToken cancellationToken = default)
        {
            var categories = await _accountRepository.GetAllAsync(cancellationToken);

            // di sini tempat aturan aplikasi
            // contoh filtering / sorting dll
            return categories
                .OrderBy(c => c.AccountName)
                .Select(c => new AccountDTO(c.Id, c.AccountName))
                .ToList();
        }
    }
}
