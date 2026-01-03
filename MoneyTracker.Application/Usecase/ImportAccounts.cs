using MoneyTracker.Domain.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Application.Usecase
{
    public class ImportAccounts
    {
        private readonly IReadCsvAccounts read;

        public ImportAccounts(IReadCsvAccounts read)
        {
            this.read = read;
        }

        public async Task ExecuteAsync(string path, CancellationToken cancellationToken = default)
        {
            await read.ReadAccounts(path);
        }
    }
}
