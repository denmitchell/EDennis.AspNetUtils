using Xunit.Abstractions;

namespace EDennis.AspNetUtils
{
    public interface ICrudService<TEntity>
        where TEntity : class
    {
        Task<TEntity> CreateAsync(TEntity input);
        Task<TEntity> UpdateAsync(TEntity input, params object[] id);
        Task<TEntity> DeleteAsync(params object[] id);
        Task<TEntity> FindAsync(params object[] id);
        Task<TEntity> FindRequiredAsync(params object[] id);

        Task<(List<TEntity> Data, int Count)> GetAsync(string where = null, object[] whereArgs = null, string orderBy = null, int? skip = null, int? take = null, CountType countType = CountType.None, string include = null, bool asNoTracking = true);
        Task<(List<dynamic> Data, int Count)> GetAsync(string select, string where = null, object[] whereArgs = null, string orderBy = null, int? skip = null, int? take = null, CountType countType = CountType.None, string include = null, bool asNoTracking = true);

        Task<IEnumerable<TEntity>> GetModifiedAsync(DateTime asOf);

        Task EnableTestAsync(ITestOutputHelper output = null);

    }
}