using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using MoneyTracker.WinUI.ViewModel;
using System;
using Microsoft.UI.Xaml.Controls;
using MoneyTracker.Application.DTO;
using MoneyTracker.WinUI.View.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MoneyTracker.WinUI.View
{
    public sealed partial class DashboardWindow : Window
    {
        public DashboardWindowViewModel _viewModel;

        public DashboardWindow(DashboardWindowViewModel vm)
        {
            _viewModel = vm ?? throw new ArgumentNullException(nameof(vm));
            this.InitializeComponent();
            Content = new DashboardPage(_viewModel,AppHostManager.AppHost.Services.GetRequiredService<ILogger<DashboardPage>>());
            //splitView.DataContext = _viewModel;

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);

            // 2. Dapatkan AppWindow
            AppWindow appWindow = AppWindow.GetFromWindowId(myWndId);

            // 3. Atur Presenter ke FullScreen
            if (appWindow != null)
            {
                appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            }

            

        }

        //private void CategoriesList_ItemClick(object sender, Microsoft.UI.Xaml.Controls.ItemClickEventArgs e)
        //{
        //    // Cek apakah item yang diklik sama dengan item yang sedang aktif/terpilih
        //    if (CategoriesList.SelectedItem == e.ClickedItem)
        //    {
        //        // Jika sama, hapus seleksi (unfocus secara visual seleksi)
        //        CategoriesList.SelectedItem = null;
        //    }
        //    else
        //    {
        //        // Jika beda, biarkan ListView menangani seleksi ke item baru
        //        CategoriesList.SelectedItem = e.ClickedItem;
        //    }
        //}

        //private void CategoriesListSettings_ItemClick(object sender, ItemClickEventArgs e)
        //{
        //    var category = e.ClickedItem as CategoryDTO;
        //    if (category != null)
        //    {
        //        _viewModel.ToggleCategoryDisplay(category);
        //    }
        //}

        //private void CategoriesListSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    // Ketika user men-check
        //    foreach (CategoryDTO cat in e.AddedItems)
        //        cat.IsDisplay = true;

        //    // Ketika user men-uncheck
        //    foreach (CategoryDTO cat in e.RemovedItems)
        //        cat.IsDisplay = false;
        //}

        //// Setelah Categories dimuat, set SelectedItems sesuai IsDisplay
        //private void CategoriesAccountsListSettings_Loaded(object sender, RoutedEventArgs e)
        //{
        //    CategoriesListSettings.SelectedItems.Clear();
        //    foreach (var category in _viewModel.Categories)
        //    {
        //        if (category.IsDisplay)
        //            CategoriesListSettings.SelectedItems.Add(category);
        //    }

        //    AccountsListSettings.SelectedItems.Clear();
        //    foreach(var acc in _viewModel.Accounts)
        //    {
        //        if (acc.IsDisplay)
        //            AccountsListSettings.SelectedItems.Add(acc);
        //    }
        //}

        //private void AccountsListSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    // Ketika user men-check
        //    foreach (AccountDTO cat in e.AddedItems)
        //        cat.IsDisplay = true;

        //    // Ketika user men-uncheck
        //    foreach (AccountDTO cat in e.RemovedItems)
        //        cat.IsDisplay = false;
        //}

        //private void AccountsListSettings_ItemClick(object sender, ItemClickEventArgs e)
        //{
        //    var acc = e.ClickedItem as AccountDTO;
        //    if (acc != null)
        //    {
        //        _viewModel.ToggleAccountsDisplay(acc);
        //    }
        //}

        //public void SyncSelectedAllAccounts(ListView listView)
        //{
        //    listView.SelectedItems.Clear();
        //    foreach (var acc in _viewModel.Accounts)
        //    {
        //        listView.SelectedItems.Add(acc);
        //    }
        //}

        //public void SyncSelectedAllCategories(ListView listView)
        //{
        //    listView.SelectedItems.Clear();
        //    foreach (var cat in _viewModel.Categories)
        //    {
        //        listView.SelectedItems.Add(cat);
        //    }
        //}


        //private void selectAllDisplayDataButton_Click(object sender, RoutedEventArgs e)
        //{
        //    SyncSelectedAllAccounts(AccountsListSettings);
        //    SyncSelectedAllCategories(CategoriesListSettings);
        //    _viewModel.SyncDataSplitView();
        //}

        //private void resetAllDisplayDataButton_Click(object sender, RoutedEventArgs e)
        //{
        //    ResetSelectedAllAccounts(AccountsListSettings);
        //    ResetSelectedAllCategories(CategoriesListSettings);
        //    _viewModel.SyncDataSplitView();
        //}

        //private void ResetSelectedAllCategories(ListView categoriesListSettings)
        //{
        //    categoriesListSettings.SelectedItems.Clear();
        //}

        //private void ResetSelectedAllAccounts(ListView accountsListSettings)
        //{
        //    accountsListSettings.SelectedItems.Clear();
        //}

        //private async void ImportCategoriesMFItem_Click(object sender, RoutedEventArgs e)
        //{
        //    var picker = new Windows.Storage.Pickers.FileOpenPicker();
        //    picker.FileTypeFilter.Add(".csv");
        //    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        //    WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        //    var file = await picker.PickSingleFileAsync();
        //    _viewModel.ReadCsvCategories(file);
        //}

        //private async void ImportAccountsMFItem_Click(object sender, RoutedEventArgs e)
        //{
        //    var picker = new Windows.Storage.Pickers.FileOpenPicker();
        //    picker.FileTypeFilter.Add(".csv");
        //    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        //    WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        //    var file = await picker.PickSingleFileAsync();
        //    _viewModel.ReadCsvAccounts(file);
        //}

        //private async void ImportTransactionsMFItem_Click(object sender, RoutedEventArgs e)
        //{
        //    var picker = new Windows.Storage.Pickers.FileOpenPicker();
        //    picker.FileTypeFilter.Add(".csv");
        //    var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        //    WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        //    var file = await picker.PickSingleFileAsync();
        //    _viewModel.ReadCsvTransactions(file);
        //}
    }
}
