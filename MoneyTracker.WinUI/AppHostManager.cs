using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoneyTracker.Application.Command;
using MoneyTracker.Application.Usecase;
using MoneyTracker.Infrastructure;
using MoneyTracker.WinUI.Services;
using MoneyTracker.WinUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.WinUI
{
    internal class AppHostManager
    {
        public static IHost AppHost { get; private set; } = default!;
        public static async Task StartAsync()
        {
            AppHost = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IAppControlService, AppControlServices>();
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
                //Dashboard Window
                services.AddTransient<DashboardWindowViewModel>();

                //UseCase
                services.AddTransient<GetAccounts>(); 

                //Infrastructure Layer
                services.AddInfrastructure();
            })
            .Build();

            await AppHost.StartAsync();
        }

        public static async Task RestartAsync()
        {
            if (AppHost is not null)
                await AppHost.StopAsync();

            await StartAsync();
        }
    }
}
