using System.Diagnostics.CodeAnalysis;

namespace EDennis.AspNetUtils
{
    public partial class CountCache<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Internal class for holding the cached record count and last calculated date.
        /// The IEqualityComparer is used to simplify comparison of objects
        /// </summary>
        public class CountAndDate : IEqualityComparer<CountAndDate>
        {
            /// <summary>
            /// The count of records (across pages)
            /// </summary>
            public int Count { get; set; }

            /// <summary>
            /// The last time the count was calculated
            /// </summary>
            public DateTime LastCalculated { get; set; }

            /// <summary>
            /// Override of Equals to compare objects in terms of values for
            /// <see cref="Count"/> and <see cref="LastCalculated"/>
            /// </summary>
            /// <param name="x">The first object's data</param>
            /// <param name="y">The second object's data</param>
            /// <returns></returns>
            public bool Equals(CountAndDate x, CountAndDate y)
                => x.Count == y.Count && x.LastCalculated == y.LastCalculated;

            /// <summary>
            /// Override of Hashcode to be based upon <see cref="Count"/> and <see cref="LastCalculated"/>
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode([DisallowNull] CountAndDate obj)
                => HashCode.Combine(obj.Count, obj.LastCalculated);
        }



    }

}
