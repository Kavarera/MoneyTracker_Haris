using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;

namespace MoneyTracker.WinUI.View
{
    public sealed partial class ConnectionWindow : Window
    {
        public ConnectionViewModel viewModel { get; } = App.Services.GetRequiredService<ConnectionViewModel>();
        public ConnectionWindow()
        {
            this.InitializeComponent();
            // Kita tunggu sampai StackPanel 'InnerContent' selesai di-render
            InnerContent.Loaded += (s, e) =>
            {
                ResizeToContent();
            };
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
            if (sender is Button btn)
            {
                try
                {
                    btn.IsEnabled = false;
                    await viewModel.TestConnectionAsync();

                    // With the following corrected line:
                    StatusMessage.Foreground = new SolidColorBrush(Colors.Green);
                }
                catch (Exception ex)
                {
                    viewModel.StatusMessage = ex.Message;
                    StatusMessage.Foreground = new SolidColorBrush(Colors.Red);
                }
                finally
                {
                    btn.IsEnabled = true;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                viewModel.Save();
                //var dW = new DashboardWindow();
                //dW.Activate();
                this.Close();
            }
        }
    }
}
