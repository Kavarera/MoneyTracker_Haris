using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MoneyTracker.Application.DTO;
using MoneyTracker.WinUI.ViewModel;
using System;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using CommunityToolkit.WinUI.UI.Controls;


namespace MoneyTracker.WinUI.View.Pages
{
    public sealed partial class DashboardPage : Page
    {

        public DashboardWindowViewModel VM => (DashboardWindowViewModel)DataContext;
        private readonly ILogger<DashboardPage> log;
        public DashboardWindowViewModel ViewModel { get; set; }
        public DashboardPage(DashboardWindowViewModel vm, ILogger<DashboardPage> logger)
        {
            ViewModel = vm ?? throw new ArgumentNullException(nameof(vm));
            log = logger;
            this.DataContext = ViewModel;
            
            this.InitializeComponent();
            
            splitView.DataContext = ViewModel;

            //IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            //WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);

            // 2. Dapatkan AppWindow
            //AppWindow appWindow = AppWindow.GetFromWindowId(myWndId);

            //// 3. Atur Presenter ke FullScreen
            //if (appWindow != null)
            //{
            //    appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
            //}



        }

        private void CategoriesList_ItemClick(object sender, Microsoft.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            // Cek apakah item yang diklik sama dengan item yang sedang aktif/terpilih
            if (CategoriesList.SelectedItem == e.ClickedItem)
            {
                // Jika sama, hapus seleksi (unfocus secara visual seleksi)
                CategoriesList.SelectedItem = null;
            }
            else
            {
                // Jika beda, biarkan ListView menangani seleksi ke item baru
                CategoriesList.SelectedItem = e.ClickedItem;
            }
        }

        private void CategoriesListSettings_ItemClick(object sender, ItemClickEventArgs e)
        {
            var category = e.ClickedItem as CategoryDTO;
            if (category != null)
            {
                ViewModel.ToggleCategoryDisplay(category);
            }
        }

        private void CategoriesListSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ketika user men-check
            foreach (CategoryDTO cat in e.AddedItems)
                cat.IsDisplay = true;

            // Ketika user men-uncheck
            foreach (CategoryDTO cat in e.RemovedItems)
                cat.IsDisplay = false;
        }

        // Setelah Categories dimuat, set SelectedItems sesuai IsDisplay
        private void CategoriesAccountsListSettings_Loaded(object sender, RoutedEventArgs e)
        {
            CategoriesListSettings.SelectedItems.Clear();
            foreach (var category in ViewModel.Categories)
            {
                if (category.IsDisplay)
                    CategoriesListSettings.SelectedItems.Add(category);
            }

            AccountsListSettings.SelectedItems.Clear();
            foreach (var acc in ViewModel.Accounts)
            {
                if (acc.IsDisplay)
                    AccountsListSettings.SelectedItems.Add(acc);
            }
        }

        private void AccountsListSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ketika user men-check
            foreach (AccountDTO cat in e.AddedItems)
                cat.IsDisplay = true;

            // Ketika user men-uncheck
            foreach (AccountDTO cat in e.RemovedItems)
                cat.IsDisplay = false;
        }

        private void AccountsListSettings_ItemClick(object sender, ItemClickEventArgs e)
        {
            var acc = e.ClickedItem as AccountDTO;
            if (acc != null)
            {
                ViewModel.ToggleAccountsDisplay(acc);
            }
        }

        public void SyncSelectedAllAccounts(ListView listView)
        {
            listView.SelectedItems.Clear();
            foreach (var acc in ViewModel.Accounts)
            {
                listView.SelectedItems.Add(acc);
            }
        }

        public void SyncSelectedAllCategories(ListView listView)
        {
            listView.SelectedItems.Clear();
            foreach (var cat in ViewModel.Categories)
            {
                listView.SelectedItems.Add(cat);
            }
        }

        private void ApplyInnerFilter_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StartDate = startDatePicker.Date?.DateTime;
            ViewModel.EndDate = endDatePicker.Date?.DateTime;
            ViewModel.InnerCategory = categoryCombo.SelectedItem as CategoryDTO;
            ViewModel.ApplyInnerFilter();
        }

        private void ResetInnerFilter_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ResetInnerFilter();
        }


        private void selectAllDisplayDataButton_Click(object sender, RoutedEventArgs e)
        {
            SyncSelectedAllAccounts(AccountsListSettings);
            SyncSelectedAllCategories(CategoriesListSettings);
            ViewModel.SyncDataSplitView();
        }

        private async void resetAllDisplayDataButton_Click(object sender, RoutedEventArgs e)
        {
            ResetSelectedAllAccounts(AccountsListSettings);
            ResetSelectedAllCategories(CategoriesListSettings);
            ViewModel.SyncDataSplitView();
            //AccountsList.SelectedItems.Clear();
            //CategoriesList.SelectedItems.Clear();
            await ViewModel.LoadAsync();
        }

        private void ResetSelectedAllCategories(ListView categoriesListSettings)
        {
            categoriesListSettings.SelectedItems.Clear();
        }

        private void ResetSelectedAllAccounts(ListView accountsListSettings)
        {
            accountsListSettings.SelectedItems.Clear();
        }

        private async void ImportCategoriesMFItem_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.FileTypeFilter.Add(".csv");
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            var file = await picker.PickSingleFileAsync();
            ViewModel.ReadCsvCategories(file);
        }

        private async void ImportAccountsMFItem_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.FileTypeFilter.Add(".csv");
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            var file = await picker.PickSingleFileAsync();
            ViewModel.ReadCsvAccounts(file);
        }

        private async void ImportTransactionsMFItem_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.FileTypeFilter.Add(".csv");
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            var file = await picker.PickSingleFileAsync();
            ViewModel.ReadCsvTransactions(file);
        }

        private void TransactionDataGrid_PreparingCellForEdit(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridPreparingCellForEditEventArgs e)
        {
            log.LogInformation("1");
            if (e.Column.Header?.ToString() != "Category")
                return;

            
            log.LogInformation("2");
            
            var presenter = e.EditingElement as ContentPresenter;
            if (presenter == null) return;

            var root = presenter.Content as FrameworkElement;
            if (root == null)
                return;

            var combo = FindChildOfType<ComboBox>(root);

            log.LogInformation("3");
            if (combo == null)
                return;

            // 1 ISI DATA
            log.LogInformation("4");
            combo.ItemsSource = ViewModel.Categories;

            combo.DisplayMemberPath = "CategoryName";
            combo.SelectedValuePath = "Id";

            // 2 TEMUKAN ROW DATA (TransactionDTO)
            var dto = (TransactionDTO)e.Row.DataContext;

            // 3 SET CURRENT VALUE
            combo.SelectedValue = dto.CategoryId;

            // 4 REACT TO CHANGE
            combo.SelectionChanged += (s, args) =>
            {
            log.LogInformation("5");
                if (combo.SelectedValue is int id)
                {
            log.LogInformation("6");
                    dto.CategoryId = id;

                    var selected = ViewModel.Categories.FirstOrDefault(c => c.Id == id);
                    dto.CategoryName = selected?.CategoryName ?? "";
                }
            };
            log.LogInformation("7");
        }


        // Fungsi Helper untuk mencari kontrol di dalam Template
        public static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);

                if (child is T match)
                    return match;

                var result = FindChildOfType<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }

        private void DataGrid_CellEditEnded(object sender, CommunityToolkit.WinUI.UI.Controls.DataGridCellEditEndedEventArgs e)
        {
            if (e.EditAction == CommunityToolkit.WinUI.UI.Controls.DataGridEditAction.Commit)
            {
                // 1. Ambil DTO-nya dulu
                if (e.Row.DataContext is TransactionDTO editedTransaction)
                {
                    var cellContent = e.Column.GetCellContent(e.Row);
                    string header = e.Column.Header.ToString();

                    // 2. Logika manual untuk kolom yang "bandel"
                    if (header == "Transaction Date")
                    {
                        var datePicker = FindChildOfType<CalendarDatePicker>(cellContent);
                        if (datePicker?.Date != null)
                        {
                            editedTransaction.TransactionDate = datePicker.Date.Value.DateTime;
                        }
                    }
                    else if (header == "Status")
                    {
                        var comboBox = FindChildOfType<ComboBox>(cellContent);
                        if (comboBox?.SelectedItem is string newStatus)
                        {
                            log.LogInformation($"new status = {newStatus}");
                            editedTransaction.Status = newStatus.Split().First().ToString();
                        }
                    }
                    // Tambahkan logika Category/Account di sini juga jika belum jalan
                }

                // 3. JURUS PAMUNGKAS (Pindahkan ke paling bawah)
                // Reset DataContext baris ini untuk memaksa SEMUA TextBlock (Status, Date, dll) menggambar ulang
                var currentData = e.Row.DataContext;
                e.Row.DataContext = null;
                e.Row.DataContext = currentData;

                // 4. Refresh List (Opsional tapi bagus untuk totalitas)
                ViewModel.RefreshFilteredTransactions();

                log.LogInformation("Update data and UI refresh completed.");
            }

            //if (e.EditAction == CommunityToolkit.WinUI.UI.Controls.DataGridEditAction.Commit)
            //{
            //    // TRIK SAKTI: Reset DataContext baris ini agar TextBlock membaca ulang {Binding Status}
            //    var rowData = e.Row.DataContext;
            //    e.Row.DataContext = null;
            //    e.Row.DataContext = rowData;

            //    log.LogInformation("Row UI Refreshed");
            //}
            //if (e.EditAction == CommunityToolkit.WinUI.UI.Controls.DataGridEditAction.Commit)
            //{
            //    if (e.Row.DataContext is TransactionDTO editedTransaction)
            //    {
            //        // Ambil elemen visual dari sel yang baru saja diedit
            //        var cellContent = e.Column.GetCellContent(e.Row);

            //        if (e.Column.Header.ToString() == "Transaction Date")
            //        {
            //            // Ambil CalendarDatePicker dari dalam DataTemplate
            //            var datePicker = cellContent as CalendarDatePicker;

            //            // Jika datePicker null, kemungkinan dia dibungkus Panel/Grid
            //            if (datePicker == null && cellContent is FrameworkElement fe)
            //            {
            //                // Cari ke bawah hirarki visual
            //                datePicker = FindChildOfType<CalendarDatePicker>(cellContent);
            //            }

            //            if (datePicker != null && datePicker.Date.HasValue)
            //            {
            //                // AMBIL NILAI LANGSUNG DARI UI DAN MASUKKAN KE DTO
            //                editedTransaction.TransactionDate = datePicker.Date.Value.DateTime;
            //                log.LogInformation($"Berhasil dipaksa update: {editedTransaction.TransactionDate}");
            //            }
            //        }
            //    }

            //    // PENTING: Picu refresh agar TextBlock di CellTemplate baca ulang
            //    ViewModel.RefreshFilteredTransactions();
            //}
            //------

            //if (e.EditAction == CommunityToolkit.WinUI.UI.Controls.DataGridEditAction.Commit)
            //{
            //    // Cara mengambil elemen ComboBox yang sedang aktif di sel tersebut:
            //    FrameworkElement cellContent = e.Column.GetCellContent(e.Row);

            //    // Cari ComboBox di dalam content tersebut
            //    ComboBox comboBox = cellContent as ComboBox;

            //    // Jika tidak ketemu langsung (karena di dalam DataTemplate), kita cari secara manual
            //    if (comboBox == null && cellContent is Panel panel)
            //    {
            //        comboBox = panel.Children.OfType<ComboBox>().FirstOrDefault();
            //    }

            //    if (comboBox != null && e.Row.DataContext is TransactionDTO editedTransaction)
            //    {
            //        // Pastikan SelectedValue tidak null sebelum di-cast
            //        if (comboBox.SelectedValue != null)
            //        {
            //            var newCategoryId = (int)comboBox.SelectedValue;

            //            // Cari di daftar kategori asli
            //            var category = ViewModel.Categories.FirstOrDefault(c => c.Id == newCategoryId);

            //            if (category != null)
            //            {
            //                editedTransaction.CategoryId = category.Id;
            //                editedTransaction.CategoryName = category.CategoryName;

            //                log.LogInformation($"Berhasil update ke: {category.CategoryName}");
            //            }
            //        }
            //    }

            //    // Paksa refresh UI agar TextBlock di CellTemplate muncul dengan nama baru
            //    ViewModel.RefreshFilteredTransactions();
            //}
        }

        private void CalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            // Cari data item yang sedang dipegang oleh baris ini
            if (sender.DataContext is TransactionDTO editedTransaction && args.NewDate.HasValue)
            {
                // Update langsung ke DTO-nya saat itu juga
                editedTransaction.TransactionDate = args.NewDate.Value.DateTime;
                log.LogInformation($"DateChanged Triggered: {editedTransaction.TransactionDate}");
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.DataContext is TransactionDTO editedTransaction)
            {
                // Ambil string yang dipilih ("Reconciled" atau "Unreconciled")
                if (comboBox.SelectedItem is string newStatus)
                {
                    editedTransaction.Status = newStatus.Split().First().ToString();
                    log.LogInformation($"Status manual update: {newStatus}");
                }
            }
        }
    }
}
