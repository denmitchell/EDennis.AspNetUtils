using Xunit;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Xunit utilities
    /// </summary>
    public static class XunitUtils
    {
        /// <summary>
        /// Convenience method for asserting that a collection contains the provided
        /// Ids in order.  This is helpful for determining if any new records were
        /// created or existing records deleted
        /// </summary>
        /// <typeparam name="TEntity">The entity class</typeparam>
        /// <param name="recs">The data records</param>
        /// <param name="ids">The expected Ids</param>
        public static void AssertOrderedIds<TEntity>(IEnumerable<TEntity> recs, params int[] ids)
            where TEntity : class, IHasIntegerId
        {
            Action<TEntity>[] asserts = ids.ToArray()
                .Select(i => {
                    return (Action<TEntity>)(actual => Assert.Equal(i, actual.Id));
                }).ToArray();

            Assert.Collection(recs, asserts);
        }


        /// <summary>
        /// Convenience method for asserting that a collection contains the provided
        /// SysGuids in order.  This is helpful for determining if any new records were
        /// created or existing records deleted
        /// </summary>
        /// <typeparam name="TEntity">The entity class</typeparam>
        /// <param name="recs">The data records</param>
        /// <param name="guidInts">The expected Guids (as integers -- converted via <see cref="GuidUtils.FromId(int)"/>)</param>
        public static void AssertOrderedSysGuids<T>(IEnumerable<T> recs, params int[] guidInts)
            where T : class, IHasSysGuid
        {
            Action<T>[] asserts = guidInts.ToArray()
                .Select(i => {
                    return (Action<T>)(actual => Assert.Equal(GuidUtils.FromId(i), actual.SysGuid));
                }).ToArray();

            Assert.Collection(recs, asserts);
        }
    }
}
