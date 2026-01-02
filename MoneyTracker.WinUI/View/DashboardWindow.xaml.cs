using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Input;
using MoneyTracker.WinUI.ViewModel;
using System;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;

namespace MoneyTracker.WinUI.View
{
    public sealed partial class DashboardWindow : Window
    {
        public DashboardWindowViewModel _viewModel;

        public DashboardWindow(DashboardWindowViewModel vm)
        {
            _viewModel = vm ?? throw new ArgumentNullException(nameof(vm));
            this.InitializeComponent();
            splitView.DataContext = _viewModel;

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

        private void CategoriesList_ItemClick(object sender, Microsoft.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            // Cek apakah item yang diklik sama dengan item yang sedang aktif/terpilih
            if (CategoriesList.SelectedItem == e.ClickedItem)
            {
                // Jika sama, hapus seleksi (unfocus secara visual seleksi)
                CategoriesList.SelectedItem = null;
                FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
            }
            else
            {
                // Jika beda, biarkan ListView menangani seleksi ke item baru
                CategoriesList.SelectedItem = e.ClickedItem;
            }
        }
    }
}
