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
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
                options.EnableSensitiveDataLogging();
            });

            services.AddTransient<IConnectionDbTest, ConnectionDbTest>();
            return services;
        }
    }
}
