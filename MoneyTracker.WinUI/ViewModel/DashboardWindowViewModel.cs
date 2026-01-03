using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using MoneyTracker.Application.DTO;
using MoneyTracker.Application.Usecase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyTracker.WinUI.ViewModel
{
    public partial class DashboardWindowViewModel : ObservableObject
    {
        private readonly ILogger<DashboardWindowViewModel> _log;

        private readonly GetAccounts _getAccounts;
        private readonly GetCategories _getCategories;
        private readonly ImportCategories _importCategories;

        public ObservableCollection<AccountDTO> Accounts { get; } = new();
        public ObservableCollection<CategoryDTO> Categories { get; } = new();
        public IEnumerable<CategoryDTO> DisplayCategories => Categories.Where(c => c.IsDisplay);
        public IEnumerable<AccountDTO> DisplayAccounts => Accounts.Where(c => c.IsDisplay);

        [ObservableProperty] private bool _isSplitPaneOpen = true;

        [ObservableProperty] private bool _isLoadingData = false;

        public bool IsNotLoadingData => !IsLoadingData;

        partial void OnIsLoadingDataChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotLoadingData));
        }


        public DashboardWindowViewModel(ILogger<DashboardWindowViewModel> logger, GetAccounts getAccounts, GetCategories getCategories, ImportCategories importCategories)
        {
            _log = logger;
            _getAccounts = getAccounts;
            _getCategories = getCategories;
            _importCategories = importCategories;
        }

        [RelayCommand]
        public void ToggleSplitPane()
        {
            _log.LogInformation("Toggle Split Pane Changed");
            IsSplitPaneOpen = !IsSplitPaneOpen;
        }

        public void SyncDataSplitView()
        {
            OnPropertyChanged(nameof(DisplayAccounts));
            OnPropertyChanged(nameof(DisplayCategories));
        }

        public void ToggleCategoryDisplay(CategoryDTO category)
        {
            category.IsDisplay = !category.IsDisplay;
            OnPropertyChanged(nameof(DisplayCategories));
        }

        public void ToggleAccountsDisplay(AccountDTO acc)
        {
            acc.IsDisplay = !acc.IsDisplay;
            OnPropertyChanged(nameof(DisplayAccounts));
        }

        public async Task LoadAsync()
        {
            IsLoadingData = true;
            try
            {
                _log.LogInformation("Loading accounts");

                var items = await _getAccounts.ExecuteAsync();
                var items2 = await _getCategories.ExecuteAsync();
                IsLoadingData = false;
                Accounts.Clear();
                Categories.Clear();
                foreach (var item in items)
                {
                    Accounts.Add(item);
                }
                foreach (var item in items2)
                {
                    Categories.Add(item);
                }

                // beri tahu UI bahwa DisplayCategories DisplayAccounts berubah
                OnPropertyChanged(nameof(DisplayCategories));
                OnPropertyChanged(nameof(DisplayAccounts));
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

        public async void ReadCsv(Windows.Storage.StorageFile file)
        {
            await _importCategories.ExecuteAsync(file.Path.ToString());
            //then refresh the data
        }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
