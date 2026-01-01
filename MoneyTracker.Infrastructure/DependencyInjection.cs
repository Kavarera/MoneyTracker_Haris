using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoneyTracker.Application.Interface;
using MoneyTracker.Infrastructure.Persistence;
using MoneyTracker.Infrastructure.Service;

namespace MoneyTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString="")
        {

            services.AddSingleton<IConnectionDatabaseProvider>(_=>new ConnectionDatabaseProvider("",5432,"","",""));

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                var store = sp.GetRequiredService<IConnectionDatabaseProvider>();
                options.UseNpgsql(store.GetConnectionString());
                options.EnableSensitiveDataLogging();
            });

            services.AddTransient<IConnectionDbTest, ConnectionDbTest>();
            return services;
        }
    }
}
