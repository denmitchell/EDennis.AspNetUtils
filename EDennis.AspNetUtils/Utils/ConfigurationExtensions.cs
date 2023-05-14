using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;

namespace EDennis.AspNetUtils
{

    /// <summary>
    /// Various helpful extension methods
    /// </summary>
    public static class ConfigurationExtensions
    {

        /// <summary>
        /// Attempts to bind a value or object to configuration, and
        /// can throw an exception upon failure to find the key.
        /// </summary>
        /// <typeparam name="T">The target type to bind to</typeparam>
        /// <param name="config">The IConfiguration instance</param>
        /// <param name="key">The configuration key</param>
        /// <param name="checkIfDefault">whether to check for the existence
        /// of the key, if the binding process returns a default value</param>
        /// <param name="logger">The logger to use for error logging</param>
        public static T GetOrThrow<T>(this IConfiguration config,
            string key, bool checkIfDefault = true, ILogger logger = null)
        {

            T result;

            if (IsSimpleType(typeof(T)))
                //bind to simple value
                result = config.GetValue<T>(key);
            else
                //bind to object
                result = config.GetSection(key).Get<T>();

            //if value is default value && checkIfDefault == true, make sure
            //that the key exists
            if (EqualityComparer<T>.Default.Equals(result, default) && checkIfDefault)
            {
                if (config is IConfigurationRoot root)
                {
                    if (root.ContainsKey(key))
                        return result;
                    else
                        Throw(key, typeof(T), logger); //key doesn't exist
                }
            }

            return result;
        }


        /// <summary>
        /// Binds and configures options for a particular type.  The
        /// deserialized options are returned through the settings out parameter.
        /// The options are configured in DI so that the DI container can
        /// inject instances of IOptionsMonitor{T}.
        /// </summary>
        /// <typeparam name="T">The model class for the settings</typeparam>
        /// <param name="services">The IServiceCollection instance</param>
        /// <param name="config">The IConfiguration instance</param>
        /// <param name="configKey">The key to the relevant section of configuration</param>
        /// <param name="settings">The resulting settings object after binding</param>
        /// <returns></returns>
        public static IServiceCollection BindAndConfigure<T>(this IServiceCollection services, IConfiguration config, string configKey, out T settings)
            where T : class, new()
        {
            settings = new T();
            var configSection = config.GetSection(configKey);
            configSection.Bind(settings);

            services.Configure<T>(configSection.Bind);

            return services;
        }


        /// <summary>
        /// Whether a configuration key exists
        /// </summary>
        /// <param name="config">The root configuration (downcast from IConfiguration,
        /// if needed)</param>
        /// <param name="key">The key to check</param>
        /// <returns></returns>
        public static bool ContainsKey(this IConfigurationRoot config, string key)
        {

            //get all the provider sources contributing to configuration
            var providers = config.Providers.ToList();

            //iterate over the provider sources, looking for the key
            for (var i = 0; i < providers.Count; i++)
            {
                var provider = providers[i];
                //if the key exists for a parent section, return true
                if (provider.GetChildKeys(Array.Empty<string>(), key).Any())
                {
                    return true;
                    //if the key exists for a value, return true
                }
                else if (provider.TryGet(key, out string _))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Helper method for throwing and logging an exception 
        /// </summary>
        /// <param name="key">The configuation key to log</param>
        /// <param name="type">The target object's type</param>
        /// <param name="logger">The logger to log to</param>
        /// <returns></returns>
        private static string Throw(string key, Type type, ILogger logger = null)
        {
            var ex = new ApplicationException($"Could not bind key '{key}' " +
                $"in Configuration to object of type {CSharpName(type)}");

            if (logger != null)
                logger.LogError(ex, ex.Message);

            throw ex;
        }


        /// <summary>
        /// from https://stackoverflow.com/a/21269486
        /// Get full type name with full namespace names
        /// </summary>
        /// <param name="type">
        /// The type to get the C# name for (may be a generic type or a nullable type).
        /// </param>
        /// <returns>
        /// Full type name, fully qualified namespaces
        /// </returns>
        public static string CSharpName(this Type type)
        {
            Type nullableType = Nullable.GetUnderlyingType(type);
            string nullableText;
            if (nullableType != null)
            {
                type = nullableType;
                nullableText = "?";
            }
            else
            {
                nullableText = string.Empty;
            }

            if (type.IsGenericType)
            {
                return string.Format(
                    "{0}<{1}>{2}",
                    type.Name[..type.Name.IndexOf('`')],
                    string.Join(", ", type.GetGenericArguments().Select(ga => ga.CSharpName())),
                    nullableText);
            }

            return type.Name switch
            {
                "String" => "string",
                "Int32" => "int" + nullableText,
                "Decimal" => "decimal" + nullableText,
                "Object" => "object" + nullableText,
                "Void" => "void" + nullableText,
                _ => (string.IsNullOrWhiteSpace(type.FullName) ? type.Name : type.FullName) + nullableText,
            };
        }


        /// <summary>
        /// From System.Data.Linq
        /// Returns true if the type is one of the built in simple types.
        /// </summary>
        internal static bool IsSimpleType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = type.GetGenericArguments()[0];

            if (type.IsEnum)
                return true;

            if (type == typeof(Guid))
                return true;

            TypeCode tc = Type.GetTypeCode(type);
            switch (tc)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Char:
                case TypeCode.String:
                case TypeCode.Boolean:
                case TypeCode.DateTime:
                    return true;
                case TypeCode.Object:
                    return (typeof(TimeSpan) == type) || (typeof(DateTimeOffset) == type);
                default:
                    return false;
            }
        }

    }

}
