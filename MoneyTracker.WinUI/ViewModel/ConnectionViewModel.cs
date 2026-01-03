using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using MoneyTracker.Application.Command;
using MoneyTracker.Application.Interface;
using System;
using System.Threading.Tasks;

namespace MoneyTracker.WinUI.ViewModel
{
    public partial class ConnectionViewModel : ObservableObject
    {
        private readonly IConnectionDatabaseProvider _connectionDatabaseProvider;
        private readonly IAppControlService _appControlService;
        private readonly IConnectionDbTest _connectionDbTest;
        private readonly ILogger<ConnectionViewModel> _logger;

        [ObservableProperty] private string _host = "localhost";
        [ObservableProperty] private double _port = 5432; // NumberBox menggunakan double
        [ObservableProperty] private string _database = "moneytracker2";
        [ObservableProperty] private string _username = "postgres";
        [ObservableProperty] private string _password = "123456";
        [ObservableProperty] private string _statusMessage;

        public ConnectionViewModel(IConnectionDatabaseProvider connectionDatabaseProvider, 
            IAppControlService appControlService, 
            IConnectionDbTest connectionDbTest,
            ILogger<ConnectionViewModel> log)
        {
            _connectionDatabaseProvider = connectionDatabaseProvider;
            _appControlService = appControlService;
            _connectionDbTest = connectionDbTest;
            _logger = log;
        }

        public bool SaveConnection()
        {
            _connectionDatabaseProvider.Set(_host, _port.ToString(), _database, _username, _password);
            return true;
        }

        public async Task<bool> TestConnection()
        {
            _logger.LogInformation("Test Connection Started");
            var res = await _connectionDbTest.TestConnectionAsync(_host, Int32.Parse(_port.ToString()), _database, _username, _password);
            StatusMessage = res.Message;
            return res.Success;
        }
    }
}
