using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MoneyTracker.WinUI.ViewModel
{
    public partial class DashboardWindowViewModel : ObservableObject
    {
        private readonly ILogger<DashboardWindowViewModel> _log;

        [ObservableProperty] private bool _isSplitPaneOpen = true;

        public DashboardWindowViewModel(ILogger<DashboardWindowViewModel> logger)
        {
            _log = logger;
        }

        [RelayCommand]
        public void ToggleSplitPane()
        {
            _log.LogInformation("Toggle Split Pane Changed");
            IsSplitPaneOpen = !IsSplitPaneOpen;
        }
    }
}
