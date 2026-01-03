using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MoneyTracker.Application.Interface;
using MoneyTracker.Domain.Interface;
using MoneyTracker.Infrastructure.ImplementRepository;
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

            services.AddSingleton<IConnectionDbTest, ConnectionDbTest>();

            //implement repo
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ITransactionsRepository, TransactionRepository>();
            services.AddSingleton<IReadCsvCategories,ReadCsvCategoriesService>();
            services.AddSingleton<IReadCsvAccounts, ReadCsvAccountsService>();
            services.AddSingleton<IReadCsvTransactions, ReadCsvTransactionsService>();

            return services;
        }
    }
}
