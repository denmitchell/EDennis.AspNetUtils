using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.CommandLine;

namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArgsController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ArgsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public Dictionary<string,string> Get()
        {
#if DEBUG
            var args = GetCommandLineArguments(_config);
            return args;
#else
            return new Dictionary<string,string>();
#endif
        }



        /// <summary>
        /// Gets command-line arguments from configuration
        /// </summary>
        /// <param name="config">the configuration</param>
        /// <returns>dictionary of command-line arguments</returns>
        public static Dictionary<string, string> GetCommandLineArguments(IConfiguration config)
        {
            var args = GetConfiguration(config, typeof(CommandLineConfigurationProvider))
                .ToDictionary(x => x.Key, x => x.Value);
            return args;
        }

        /// <summary>
        /// Gets a configuration enumeration for a particular
        /// provider
        /// </summary>
        /// <param name="config">root configuration</param>
        /// <param name="providerType">provider type</param>
        /// <returns>enumerable of configuration sections</returns>
        public static IEnumerable<IConfigurationSection> GetConfiguration(IConfiguration config, Type providerType)
        {
            var root = config as IConfigurationRoot;
            string path = null;

            //filter the list of providers
            var providers = root.Providers.Where(p => p.GetType() == providerType);

            //build the configuration enumeration for the provider.
            //use the Aggregate extension method to build the 
            //configuration cumulatively.
            //(see https://github.com/aspnet/Configuration/blob/master/src/Config/ConfigurationRoot.cs)
            var entries = providers
                .Aggregate(Enumerable.Empty<string>(),
                    (seed, source) => source.GetChildKeys(seed, path))
                .Distinct()
                .Select(key => GetSection(root, path == null ? key : ConfigurationPath.Combine(path, key)));
            return entries;
        }


        /// <summary>
        /// Gets a configuartion section
        /// </summary>
        /// <param name="root">root configuration</param>
        /// <param name="key">key for the section</param>
        /// <returns>the configuratio section</returns>
        public static IConfigurationSection GetSection(IConfigurationRoot root, string key)
        {
            return new ConfigurationSection(root, key);
        }


    }
}
