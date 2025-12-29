using CommunityToolkit.Mvvm.ComponentModel;

namespace MoneyTracker.WinUI.ViewModel
{
    public partial class ConnectionViewModel : ObservableObject
    {
        [ObservableProperty] private string _host = "localhost";
        [ObservableProperty] private double _port = 5432; // NumberBox menggunakan double
        [ObservableProperty] private string _database = "moneytracker";
        [ObservableProperty] private string _username = "postgres";
        [ObservableProperty] private string _password = "123456";
        [ObservableProperty] private string _statusMessage;
    }
}
