using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Application.Command
{
    public interface IAppControlService
    {
        Task RestartAsync();
    }
}
