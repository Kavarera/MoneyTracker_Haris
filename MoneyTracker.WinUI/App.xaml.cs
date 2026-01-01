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
        public static IHost? Host;
        public App()
        {
            this.InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddConsole();
                    builder.AddDebug();

                });
                // register services / repos / usecases di sini
                
                //Application layer
                //ViewModels
                // Connection Window
                services.AddTransient<ConnectionViewModel>();


                //Infrastructure Layer
                services.AddInfrastructure();
            })
            .Build();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new ConnectionWindow(Host.Services.GetRequiredService<ConnectionViewModel>());
            m_window.Activate();
        }

        private Window? m_window;
    }
}
