using MoneyTracker.Application.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.WinUI.Services
{
    internal class AppControlServices : IAppControlService
    {
        public Task RestartAsync()
        {
            return AppHostManager.RestartAsync();
        }
    }
}
