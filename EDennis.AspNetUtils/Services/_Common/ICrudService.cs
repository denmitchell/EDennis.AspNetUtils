using Xunit.Abstractions;

namespace EDennis.AspNetUtils
{
    public interface ICrudService<TEntity>
        where TEntity : class
    {
        TEntity Create(TEntity input);
        TEntity Update(TEntity input, params object[] id);
        TEntity Delete(params object[] id);
        TEntity Find(params object[] id);

        (List<TEntity> Data, int Count) Get(string where = null, object[] whereArgs = null, string orderBy = null, int? skip = null, int? take = null, CountType countType = CountType.None, string include = null, bool asNoTracking = true);
        (List<dynamic> Data, int Count) Get(string select, string where = null, object[] whereArgs = null, string orderBy = null, int? skip = null, int? take = null, CountType countType = CountType.None, string include = null, bool asNoTracking = true);

        IEnumerable<TEntity> GetModified(DateTime asOf);

        void EnableTest(ITestOutputHelper output = null);

    }
}