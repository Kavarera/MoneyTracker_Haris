using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MoneyTracker.WinUI.ViewModel;
using System;
using System.Net;
using Windows.Networking.Connectivity;


namespace MoneyTracker.WinUI
{
    
    public partial class App : Application
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

                RegisteredServices(services);
            })
            .Build();
        }


        private void RegisteredServices(IServiceCollection services)
        {
            //DB Session

            //App Interfaces

            //EF Core Factory

            //ViewModels
            services.AddTransient<ConnectionViewModel>();

            services.AddLogging(builder =>
            {
                builder.AddDebug();
            });

        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window? m_window;
    }
}
