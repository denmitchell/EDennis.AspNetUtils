using SqlServerExceptions = EntityFramework.Exceptions.SqlServer.ExceptionProcessorExtensions;
using SqliteExceptions = EntityFramework.Exceptions.Sqlite.ExceptionProcessorExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using Xunit.Abstractions;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Singleton service used to construct production and test DbContexts.  The class
    /// includes many static helper methods.  For simplicity, there is only support
    /// for SQL Server and SQLite In Memory.  For SQL Server, there is support for
    /// normal (framework-managed) transactions and open transactions (for testing)
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class DbContextService<TContext> 
        where TContext : DbContext
    {

        #region Variables

        /// <summary>
        /// The connection string for SQL Server
        /// </summary>
        private readonly string _sqlServerConnectionString;

        /// <summary>
        /// The key to the section in configuration that holds connection strings
        /// for each of the DbContexts.  Note that the section should be 
        /// constructed such that the name of each DbContext class is a subkey under this
        /// ConfigurationSectionKey.  For example, if there are are two DbContext classes
        /// -- an AppUserContext class and a HitContext class -- then the configuration
        /// might look like this in appsettings:
        /// <code>
        /// {
        /// ...
        ///   "DbContexts": {
        ///     "AppUserContext": "Server=(localdb)\\mssqllocaldb;Database=...",
        ///     "HitsContext": ""Server=(localdb)\\mssqllocaldb;Database=...""
        ///   }
        /// }
        /// </code>
        /// </summary>
        public string ConfigurationSectionKey { get; set; } = "DbContexts";

        #endregion
        #region Constructor

        /// <summary>
        /// Constructs a new instance of <see cref="DbContextService{TContext}"/> with the
        /// provided/injected <see cref="IConfiguration"/> instance
        /// </summary>
        /// <param name="config"></param>
        public DbContextService(IConfiguration config = null)
        {
            _sqlServerConnectionString = GetConnectionString(config, ConfigurationSectionKey);
        }

        #endregion
        #region Testing

        /// <summary>
        /// Gets a test instance of a DbContext.  At present there are three possibilities:
        /// <list type="table">
        /// NOTE: The test DbContext enables Sensitive Data Logging.
        /// </summary>
        /// <param name="output">Xunit object for directing test/logging output to a place that can be read</param>
        /// <returns></returns>
        public TContext GetTestServiceContext(ITestOutputHelper output = null)
        {
                DbConnection connection = new SqlConnection(_sqlServerConnectionString);

                var builder = new DbContextOptionsBuilder<TContext>();
                builder.UseSqlServer(connection)
                    .EnableSensitiveDataLogging();

                if (output != null)
                    builder.LogTo(output.WriteLine);

                SqlServerExceptions.UseExceptionProcessor(builder);

                connection.Open();
                var transaction = connection.BeginTransaction();

                TContext context = (TContext)Activator.CreateInstance(typeof(TContext), builder.Options);
                context.Database.UseTransaction(transaction);
                return context;

        }

        #endregion
        #region Production

        /// <summary>
        /// Gets the DbContext from Configuration
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public TContext GetDbContext(IConfiguration config)
            => GetDbContext(GetConnectionString(config, ConfigurationSectionKey));


        /// <summary>
        /// Builds a DbContext from Configuration
        /// </summary>
        /// <param name="config"><see cref="IConfiguration"/></param>
        /// <param name="sectionKey">the configuration key for the DbContexts section</param>
        /// <returns></returns>
        public static TContext GetDbContext(IConfiguration config,
            string sectionKey = "DbContexts")
            => GetDbContext(GetConnectionString(config, sectionKey));

        /// <summary>
        /// Builds a DbContextOptions object from Configuration
        /// </summary>
        /// <param name="config"><see cref="IConfiguration"/></param>
        /// <param name="sectionKey">the configuration key for the DbContexts section</param>
        /// <returns></returns>
        public static DbContextOptions<TContext> GetDbContextOptions(IConfiguration config,
            string sectionKey = "DbContexts")
            => GetDbContextOptions(GetConnectionString(config, sectionKey));


        /// <summary>
        /// Builds a DbContextOptions object from a connection string
        /// </summary>
        /// <param name="cxnString">The connection string</param>
        /// <returns></returns>
        public static DbContextOptions<TContext> GetDbContextOptions(string cxnString)
        {
            var builder = new DbContextOptionsBuilder<TContext>();
            builder.UseSqlServer(cxnString);
            SqlServerExceptions.UseExceptionProcessor(builder);

            return builder.Options;
        }

        /// <summary>
        /// Gets the connection string from Configuration
        /// </summary>
        /// <param name="config"><see cref="IConfiguration"/></param>
        /// <param name="sectionKey">the configuration key for the DbContexts section</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public static string GetConnectionString(IConfiguration config, string sectionKey = "DbContexts")
        {
            var cxnString = config.GetSection($"{sectionKey}:{typeof(TContext).Name}").Get<string>();
            if (string.IsNullOrEmpty(cxnString))
                throw new ApplicationException($"Connection string for {typeof(TContext).Name} " +
                    $"not defined in Configuration (e.g., appsettings)");

            return cxnString;
        }


        /// <summary>
        /// Gets the DbContext from a connection string
        /// </summary>
        /// <param name="cxnString">The connection string</param>
        /// <returns></returns>
        public static TContext GetDbContext(string cxnString)
        {
            var options = GetDbContextOptions(cxnString);
            TContext context = (TContext)Activator.CreateInstance(typeof(TContext), options);
            return context;
        }


        #endregion
    }
}
