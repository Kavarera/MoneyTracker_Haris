using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
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
        private readonly GetTransactions _getTransactions;
        private readonly ImportCategories _importCategories;
        private readonly ImportAccounts _importAccounts;
        private readonly ImportTransactions _importTransactions;

        public ObservableCollection<AccountDTO> Accounts { get; } = new();
        public ObservableCollection<CategoryDTO> Categories { get; } = new();
        public ObservableCollection<TransactionDTO> Transactions { get; } = new();

        

        public IEnumerable<CategoryDTO> DisplayCategories => Categories.Where(c => c.IsDisplay);
        public IEnumerable<AccountDTO> DisplayAccounts => Accounts.Where(c => c.IsDisplay);

        [ObservableProperty] private bool _isSplitPaneOpen = true;

        [ObservableProperty] private bool _isLoadingData = false;
        [ObservableProperty] private bool _isEditMode = false;
        [ObservableProperty]
        private string _editButtonContent = "Edit Mode";

        
        public void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
            EditButtonContent = IsEditMode ? "Save Changes" : "Edit Mode";
            _log.LogInformation($"Toggle Edit Mode Clicked = {IsEditMode}");

            if (!IsEditMode) SaveToDatabase();
        }

        private async void SaveToDatabase()
        {
            // Pastikan semua DateTime adalah UTC sebelum dikirim ke PostgreSQL
            foreach (var t in Transactions)
            {
                // Logika konversi Status "Reconciled" -> "R" biasanya dihandle di DB Context
                // Tapi pastikan Kind Date adalah UTC agar tidak error lagi
                t.TransactionDate = DateTime.SpecifyKind(t.TransactionDate, DateTimeKind.Utc);
            }

            //await _db.SaveChangesAsync();
            // Berikan notifikasi sukses jika perlu
        }

        public bool IsNotLoadingData => !IsLoadingData;

        partial void OnIsLoadingDataChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotLoadingData));
        }


        public DashboardWindowViewModel(ILogger<DashboardWindowViewModel> logger, 
            GetAccounts getAccounts, GetCategories getCategories, 
            ImportCategories importCategories, ImportAccounts importAccounts, ImportTransactions importTransactions, GetTransactions getTransactions)
        {
            _log = logger;
            _getAccounts = getAccounts;
            _getCategories = getCategories;
            _importCategories = importCategories;
            _importAccounts = importAccounts;
            _importTransactions = importTransactions;
            _getTransactions = getTransactions;
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
                var itemTransactions = await _getTransactions.ExecuteAsync();
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
                foreach(var item in itemTransactions)
                {
                    _log.LogInformation($"DATA TRABNSAKSI = {item.Description} - {item.Status}");
                    Transactions.Add(item);
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

        public async void ReadCsvCategories(Windows.Storage.StorageFile file)
        {
            if(file== null)
            {
                _log.LogWarning("No file selected for importing categories.");
                return;
            }

            IsLoadingData = true;
            try
            {
                await _importCategories.ExecuteAsync(file.Path.ToString());
                await LoadAsync();
            }catch(Exception ex)
            {
                _log.LogError(ex,$"Failed to import categories from CSV: {ex.Message}");
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        public async void ReadCsvAccounts(Windows.Storage.StorageFile file)
        {
            if (file == null)
            {
                _log.LogWarning("No file selected for importing categories.");
                return;
            }

            IsLoadingData = true;
            try
            {
                await _importAccounts.ExecuteAsync(file.Path.ToString());
                await LoadAsync();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Failed to import categories from CSV: {ex.Message}");
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        public async void ReadCsvTransactions(Windows.Storage.StorageFile file)
        {
            if (file == null)
            {
                _log.LogWarning("No file selected for importing categories.");
                return;
            }

            IsLoadingData = true;
            try
            {
                await _importTransactions.ExecuteAsync(file.Path.ToString());
                await LoadAsync();
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Failed to import categories from CSV: {ex.Message}");
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
