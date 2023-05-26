using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Extend this class to force the Entity Framework Core Tools to use this class
    /// for code-first data migrations.  This pattern gives the developer the most
    /// control during migrations.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class DesignTimeDbContextFactory<TContext> : IDesignTimeDbContextFactory<TContext>
        where TContext : DbContext
    {
        /// <summary>
        /// Creates a DbContext from appsettings files.  
        /// Note: Sensitive Data Logging is enabled.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public TContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? "Development";

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddJsonFile($"appsettings.Migrations.json", optional: true)
                .Build();

            var cxnString = DbContextService<TContext>.GetConnectionString(config);

            var builder = new DbContextOptionsBuilder<TContext>();
            builder.UseSqlServer(cxnString)
                .EnableSensitiveDataLogging()
                .UseExceptionProcessor();

            return (TContext)Activator.CreateInstance(typeof(TContext), builder.Options);
        }

    }
}
