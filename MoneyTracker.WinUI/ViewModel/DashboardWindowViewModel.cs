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
        private readonly GetTransactions _getTransactions;
        private readonly ImportCategories _importCategories;
        private readonly ImportAccounts _importAccounts;
        private readonly ImportTransactions _importTransactions;
        private readonly SaveTransactions _saveTransactions;

        public ObservableCollection<AccountDTO> Accounts { get; } = new();
        public ObservableCollection<CategoryDTO> Categories { get; } = new();
        public ObservableCollection<TransactionDTO> Transactions { get; } = new();
        public ObservableCollection<TransactionDTO> AllTransactions { get; } = new();
        public ObservableCollection<TransactionDTO> FilteredTransactions { get; } = new();
        public List<TransactionDTO> Snapshot { get; } = new();

        [ObservableProperty]
        private AccountDTO? _selectedAccount;

        [ObservableProperty]
        private CategoryDTO? _selectedCategory;



        public IEnumerable<CategoryDTO> DisplayCategories => Categories.Where(c => c.IsDisplay);
        public IEnumerable<AccountDTO> DisplayAccounts => Accounts.Where(c => c.IsDisplay);

        [ObservableProperty] private bool _isSplitPaneOpen = true;

        [ObservableProperty] private bool _isLoadingData = false;
        [ObservableProperty] private bool _isEditMode = false;
        [ObservableProperty]
        private string _editButtonContent = "Edit Mode";

        
        public void ToggleEditMode()
        {
            if (IsEditMode)
            {
                Snapshot.Clear();
                Snapshot.AddRange(FilteredTransactions.Select(t => t.Clone()));
            }
            IsEditMode = !IsEditMode;
            EditButtonContent = IsEditMode ? "Save Changes" : "Edit Mode";
            _log.LogInformation($"Toggle Edit Mode Clicked = {IsEditMode}");
            if (!IsEditMode) SaveToDatabase();
        }

        private async void SaveToDatabase()
        {
            var updates = new List<TransactionDTO>();
            // Pastikan semua DateTime adalah UTC sebelum dikirim ke PostgreSQL
            foreach (var t in FilteredTransactions)
            {
                // Logika konversi Status "Reconciled" -> "R" biasanya dihandle di DB Context
                // Tapi pastikan Kind Date adalah UTC agar tidak error lagi
                t.TransactionDate = DateTime.SpecifyKind(t.TransactionDate, DateTimeKind.Utc);
                var original = Snapshot.First(x => x.Id == t.Id);
                if (!AreEqual(original, t))
                {
                    updates.Add(t);
                }
            }

            if (updates.Any())
            {
                await _saveTransactions.ExecuteAsync(updates);
            }
            // refresh snapshot
            Snapshot.Clear();
            Snapshot.AddRange(AllTransactions.Select(t => t.Clone()));

            // Berikan notifikasi sukses jika perlu
            await LoadAsync();
        }

        public bool IsNotLoadingData => !IsLoadingData;

        partial void OnIsLoadingDataChanged(bool value)
        {
            OnPropertyChanged(nameof(IsNotLoadingData));
        }


        public DashboardWindowViewModel(ILogger<DashboardWindowViewModel> logger, 
            GetAccounts getAccounts, GetCategories getCategories, 
            ImportCategories importCategories, ImportAccounts importAccounts, ImportTransactions importTransactions, GetTransactions getTransactions,
            SaveTransactions saveTransactions)
        {
            _log = logger;
            _getAccounts = getAccounts;
            _getCategories = getCategories;
            _importCategories = importCategories;
            _importAccounts = importAccounts;
            _importTransactions = importTransactions;
            _getTransactions = getTransactions;
            _saveTransactions = saveTransactions;
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

        partial void OnSelectedAccountChanged(AccountDTO value)
        {
            ApplyFilters();
        }

        partial void OnSelectedCategoryChanged(CategoryDTO value)
        {
            ApplyFilters();
        }


        private void ApplyFilters()
        {
            var query = AllTransactions.AsEnumerable();

            if (SelectedAccount is not null)
                query = query.Where(t => t.AccountId == SelectedAccount.Id);

            if (SelectedCategory is not null)
                query = query.Where(t => t.CategoryId == SelectedCategory.Id);

            Transactions.Clear();
            foreach (var item in query)
                Transactions.Add(item);
            ApplyInnerFilter();
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
                AllTransactions.Clear();
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
                    //_log.LogInformation($"DATA TRABNSAKSI = {item.Description} - {item.Status}");
                    AllTransactions.Add(item);
                    Snapshot.Add(item.Clone());
                }
                ApplySidebarFilter();
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

        private void ApplySidebarFilter()
        {
            Transactions.Clear();

            var filtered = AllTransactions.AsEnumerable();

            if (SelectedAccount != null)
                filtered = filtered.Where(t => t.AccountId == SelectedAccount.Id);

            if (SelectedCategory != null)
                filtered = filtered.Where(t => t.CategoryId == SelectedCategory.Id);

            foreach (var t in filtered)
                Transactions.Add(t);

            ApplyInnerFilter(); // penting!
        }

        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public CategoryDTO? InnerCategory { get; set; }

        public void ResetInnerFilter()
        {
            StartDate = null;
            EndDate = null;
            InnerCategory = null;

            ApplyInnerFilter();
        }


        public void ApplyInnerFilter()
        {
            FilteredTransactions.Clear();

            var q = Transactions.AsEnumerable();

            if (StartDate != null)
            {
                q = q.Where(t => t.TransactionDate >= StartDate?.DateTime);
            }
                

            if (EndDate != null)
                q = q.Where(t => t.TransactionDate <= EndDate?.DateTime);

            if (InnerCategory != null)
                q = q.Where(t => t.CategoryId == InnerCategory.Id);

            foreach (var t in q)
                FilteredTransactions.Add(t);
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

        private bool AreEqual(TransactionDTO a, TransactionDTO b)
        {
            return
                a.TransactionDate == b.TransactionDate &&
                a.Description == b.Description &&
                a.Status == b.Status &&
                a.CategoryId == b.CategoryId &&
                a.Kredit == b.Kredit &&
                a.Debit == b.Debit;
        }



        internal void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
