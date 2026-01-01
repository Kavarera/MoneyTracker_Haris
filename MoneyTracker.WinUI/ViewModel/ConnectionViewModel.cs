using CommunityToolkit.Mvvm.ComponentModel;
using MoneyTracker.Application.Command;
using MoneyTracker.Application.Interface;
using System.Threading.Tasks;

namespace MoneyTracker.WinUI.ViewModel
{
    public partial class ConnectionViewModel : ObservableObject
    {
        private readonly IConnectionDatabaseProvider _connectionDatabaseProvider;
        private readonly IAppControlService _appControlService;
        [ObservableProperty] private string _host = "localhost";
        [ObservableProperty] private double _port = 5432; // NumberBox menggunakan double
        [ObservableProperty] private string _database = "moneytracker";
        [ObservableProperty] private string _username = "postgres";
        [ObservableProperty] private string _password = "123456";
        [ObservableProperty] private string _statusMessage;

        public async Task SaveConnection()
        {
            _connectionDatabaseProvider.Set(_host, _port.ToString(), _database, _username, _password);
            await _appControlService.RestartAsync();
        }
    }
}
