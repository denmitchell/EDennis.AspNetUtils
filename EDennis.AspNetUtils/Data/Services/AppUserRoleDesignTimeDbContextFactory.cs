using EDennis.AspNetUtils.Tests.BlazorSample;
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
    public abstract class AppUserRoleDesignTimeDbContextFactory<TAppUserRolesContext> : IDesignTimeDbContextFactory<TAppUserRolesContext>
    where TAppUserRolesContext : AppUserRolesContext
    {
        public virtual IEnumerable<AppRole> AppRoleData
            => DefaultTestRecords.GetAppRoles();

        public virtual IEnumerable<AppUser> AppUserData
            => DefaultTestRecords.GetAppUsers();


        /// <summary>
        /// Creates a DbContext from appsettings files.  
        /// Note: Sensitive Data Logging is enabled.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public TAppUserRolesContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? "Development";

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env}.json")
                .Build();

            var cxnString = DbContextService<AppUserRolesContext>.GetConnectionString(config);

            var builder = new DbContextOptionsBuilder<AppUserRolesContext>();
            builder.UseSqlServer(cxnString)
                .EnableSensitiveDataLogging()
                .UseExceptionProcessor();

            Action<AppUserRolesContext> modelEnhancements = c =>
            {
                c.RoleData = AppRoleData;
                c.UserData = AppUserData;
            };

            var context = (TAppUserRolesContext)Activator.CreateInstance(typeof(TAppUserRolesContext), builder.Options);
            context.RoleData = AppRoleData;
            context.UserData = AppUserData;
            return context;
        }

    }
}
