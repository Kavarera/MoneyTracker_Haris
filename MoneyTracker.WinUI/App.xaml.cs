using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using MoneyTracker.Infrastructure;
using MoneyTracker.WinUI.View;
using MoneyTracker.WinUI.ViewModel;


namespace MoneyTracker.WinUI
{
    
    public partial class App : Microsoft.UI.Xaml.Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            await AppHostManager.StartAsync();
            m_window = new ConnectionWindow(AppHostManager.AppHost.Services.GetRequiredService<ConnectionViewModel>());
            m_window.Activate();
        }

        private Window? m_window;
    }
}
