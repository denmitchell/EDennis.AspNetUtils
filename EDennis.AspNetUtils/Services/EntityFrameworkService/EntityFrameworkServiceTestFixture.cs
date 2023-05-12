using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using Xunit.Abstractions;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Used for XUnit testing of <see cref="EntityFrameworkService{TContext, TEntity}"/> operations
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class EntityFrameworkServiceTestFixture<TContext, TService, TEntity>
        where TContext : DbContext
        where TEntity : class
        where TService : EntityFrameworkService<TContext, TEntity>
    {
        /// <summary>
        /// Cached configurations for tests using this fixture
        /// </summary>
        private readonly static ConcurrentDictionary<string, IConfiguration> _configs = new();

        /// <summary>
        /// Gets a new <see cref="EntityFrameworkService{TContext, TEntity}"/> object, using the provided
        /// arguments.
        /// </summary>
        /// <param name="appsettingsFile">The configuration file</param>
        /// <param name="userName">The user name</param>
        /// <param name="role">The role of the user</param>
        /// <param name="dbContextType">The type of DbContext to use for testing</param>
        /// <param name="output">A reference to an XUnit helper object for directing output to a viewable location</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TService GetCrudService(string appsettingsFile, string userName, string role,
            ITestOutputHelper output = null)
        {

            //get the appsettings file and build configuration from it
            var appsettingsFilePath = Path.Combine(Directory.GetCurrentDirectory(), appsettingsFile);
            if (!File.Exists(appsettingsFilePath))
                throw new Exception($"Invalid appsettings path: {appsettingsFilePath}");

            if (!_configs.TryGetValue(appsettingsFile, out var config))
            {
                config = new ConfigurationBuilder()
                    .AddJsonFile(appsettingsFilePath)
                    .AddEnvironmentVariables()
                    .Build();
                _configs.TryAdd(appsettingsFile, config);
            }

            //create a test instance of CrudService dependencies
            var deps = EntityFrameworkServiceDependencies<TContext, TEntity>.GetTestInstance(config, userName, role);

            //create an instance of the Crud Service
            TService service = (TService)Activator.CreateInstance(typeof(TService), deps);

            //set the DbContext
            service.EnableTestAsync(output).Wait();

            return service;
        }

    }
}
