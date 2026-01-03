using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTracker.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        AppDbContext IDesignTimeDbContextFactory<AppDbContext>.CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Masukkan connection string PostgreSQL kamu di sini
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=moneytracker2;Username=postgres;Password=123456");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
