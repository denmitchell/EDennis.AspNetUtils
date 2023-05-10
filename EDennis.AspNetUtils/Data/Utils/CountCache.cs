using System.Text.Json;
using System.Collections.Concurrent;
using Radzen;
using System.Text.Json.Serialization;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Caches the total number of records returned by a query across
    /// pages. The cache prevents having to recalculate the record count
    /// during each page request.
    /// </summary>
    /// <typeparam name="TEntity">The cache is specific to a given model class (entity)</typeparam>
    public partial class CountCache<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// The cached data.  The key is the query's filter and order by.
        /// </summary>
        private readonly ConcurrentDictionary<string, CountAndDate> _dict
            = new ();

        /// <summary>
        /// The default expiration of cached values in seconds
        /// </summary>
        public static readonly TimeSpan DEFAULT_TOLERANCE = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Gets the total count of records for the given query.  This total will be
        /// retrieved from the cache when it has been saved already and the total 
        /// amount of time elapsed is less than that specified by the tolerance.
        /// </summary>
        /// <param name="filter">The Where clause (Dynamic Linq Expression)</param>
        /// <param name="filterArgs">The Args clause (Dynamic Linq Expression)</param>
        /// <param name="query">The source query</param>
        /// <param name="toleranceBeforeRefresh">The maximum amount of time to elapse before refreshing the cache</param>
        /// <returns></returns>
        public int GetCount(IQueryable<TEntity> query, string where = null, object[] whereArgs = null, TimeSpan? toleranceBeforeRefresh
            = null)
        {
            toleranceBeforeRefresh ??= DEFAULT_TOLERANCE;

            var key = where ?? "";

            try
            {
                key += whereArgs == null || whereArgs.Any() ? ""
                    : string.Join(',', whereArgs);
            } catch
            {
                //handle complex filter arguments (may not be needed)
                key += whereArgs == null || !whereArgs.Any() ? ""
                    : JsonSerializer.Serialize(whereArgs, new JsonSerializerOptions {
                         ReferenceHandler = ReferenceHandler.IgnoreCycles
                    });
            }



            int count;

            //try to get the record count from the cache
            if (_dict.TryGetValue(key, out CountAndDate value)) 
            {
                //if the cached value is too old then retrieve a new count and add it to the cache
                if (value.LastCalculated.Add(toleranceBeforeRefresh.Value) >= DateTime.Now)
                {
                    count = query.Count();
                    _dict.TryUpdate(key, new CountAndDate { Count = count, LastCalculated = DateTime.Now }, value);
                }
                else
                {
                    //return the cached value
                    count = value.Count;
                }
            } 
            else
            {
                //when the query is new, the cache doesn't yet have the total records;
                //get a new count and add it to the cache
                count = query.Count();
                _dict.TryAdd(key, new CountAndDate { Count = count, LastCalculated = DateTime.Now });
            }

            return count;
        }

#if DEBUG
        /// <summary>
        /// If in DEBUG, allow access to cache via a property
        /// </summary>
        public ConcurrentDictionary<string, CountAndDate> Cache => _dict;
#endif


    }

}
