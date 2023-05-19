using System.Collections.Concurrent;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Utilities supporting Types
    /// </summary>
    public static class TypeUtils
    {
        /// <summary>
        /// Cast from one type to another using a JsonSerializer.  Note that the classes do not have to
        /// be related via inheritance for this cast to work.
        /// </summary>
        /// <typeparam name="I">The Input Type</typeparam>
        /// <typeparam name="O">The Output Type</typeparam>
        /// <param name="input">The Input object</param>
        /// <returns></returns>
        public static O Cast<I,O>(I input)
        {
            var serlialized = JsonSerializer.Serialize(input, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            var deserialized = JsonSerializer.Deserialize<O>(serlialized);
            return deserialized;
        }
    }
}
