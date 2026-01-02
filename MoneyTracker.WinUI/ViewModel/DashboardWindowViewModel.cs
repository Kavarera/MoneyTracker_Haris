using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using MoneyTracker.Application.DTO;
using MoneyTracker.Application.Usecase;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MoneyTracker.WinUI.ViewModel
{
    public partial class DashboardWindowViewModel : ObservableObject
    {
        private readonly ILogger<DashboardWindowViewModel> _log;

        private readonly GetAccounts _getAccounts;

        public ObservableCollection<AccountDTO> Accounts { get; } = new();

        [ObservableProperty] private bool _isSplitPaneOpen = true;

        [ObservableProperty] private bool _isLoadingData = false;

        public bool IsNotLoadingData => !IsLoadingData;

        partial void OnIsLoadingDataChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotLoadingData));
        }


        public DashboardWindowViewModel(ILogger<DashboardWindowViewModel> logger, GetAccounts getAccounts)
        {
            _log = logger;
            _getAccounts = getAccounts;
        }

        [RelayCommand]
        public void ToggleSplitPane()
        {
            _log.LogInformation("Toggle Split Pane Changed");
            IsSplitPaneOpen = !IsSplitPaneOpen;
        }

        public async Task LoadAsync()
        {
            IsLoadingData = true;
            try
            {
                _log.LogInformation("Loading accounts");

                var items = await _getAccounts.ExecuteAsync();
                Accounts.Clear();
                foreach (var item in items)
                {
                    Accounts.Add(item);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Failed to load accounts\n{ex.Message}");
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
