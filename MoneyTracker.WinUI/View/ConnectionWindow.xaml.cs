using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MoneyTracker.WinUI.ViewModel;
using System;

namespace MoneyTracker.WinUI.View
{
    public sealed partial class ConnectionWindow : Window
    {
        public ConnectionViewModel viewModel { get; }
        public ConnectionWindow(ConnectionViewModel vm)
        {
            this.InitializeComponent();
            // Kita tunggu sampai StackPanel 'InnerContent' selesai di-render
            InnerContent.Loaded += (s, e) =>
            {
                ResizeToContent();
            };
            viewModel = vm ?? throw new ArgumentNullException(nameof(vm)); ;
        }

        private void ResizeToContent()
        {
            // 1. Dapatkan Handle Window (hWnd)
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            if (appWindow != null)
            {
                // 2. Ambil ukuran asli dari konten (InnerContent)
                // Kita tambahkan ekstra pixel untuk margin/border window dan title bar
                double width = InnerContent.ActualWidth + 40;
                double height = InnerContent.ActualHeight + 80;

                // 3. Eksekusi Resize
                appWindow.Resize(new Windows.Graphics.SizeInt32((int)width, (int)height));

                // Opsional: Ketengahkan window setelah resize
                CenterWindow(appWindow);
            }
        }

        private void CenterWindow(Microsoft.UI.Windowing.AppWindow appWindow)
        {
            var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(appWindow.Id, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
            if (displayArea != null)
            {
                var centeredPosition = appWindow.Position;
                centeredPosition.X = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
                centeredPosition.Y = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;
                appWindow.Move(centeredPosition);
            }
        }

        private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                viewModel.Password = pb.Password;
            }
        }

        private async void TestConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            TestConnectionButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            var result = await viewModel.TestConnection();
            if (result)
            {
                StatusMessage.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                StatusMessage.Foreground = new SolidColorBrush(Colors.Red);
            }
            TestConnectionButton.IsEnabled = true;
            SaveButton.IsEnabled = true;

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = false;

            var ok = viewModel.SaveConnection();

            if (!ok)
            {
                SaveButton.IsEnabled = true;
                return;
            }

            var dashboard = new DashboardWindow(
                AppHostManager.AppHost.Services.GetRequiredService<DashboardWindowViewModel>()
            );

            dashboard.Activate();

            // tutup window ini
            this.Close();
        }
    }
}
