using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace EDennis.AspNetUtils.Data.Services
{
    public class ApiClientService<TEntity> : ICrudService<TEntity> where TEntity : class
    {

        /// <summary>
        /// The HttpClient for communicating with API endpoints
        /// </summary>
        public HttpClient HttpClient { get; }

        
        public virtual string ConfigKey { get; }
        public virtual string ControllerPath { get; }

        /// <summary>
        /// Constructs a new QueryApiClient with the provided IHttpClientFactory.
        /// </summary>
        /// <param name="clientFactory">factory for creating a named HttpClient</param>
        public ApiClientService(IHttpClientFactory clientFactory, IConfiguration config)
        {
            HttpClient = clientFactory.CreateClient(typeof(TEntity).Name);
            ConfigKey ??= $"ApiClients:{typeof(TEntity).Name}";
            ControllerPath ??= $"api/{typeof(TEntity).Name}Controller";
        }


        private static StringContent GetContent(TEntity entity)
            => new(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");

        private static async Task<TEntity> GetEntityAsync(HttpResponseMessage response, string endpoint)
        {
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Could not retrieve entity at {endpoint}");

            var body = await response.Content.ReadAsStringAsync();
            var entity = JsonSerializer.Deserialize<TEntity>(body);
            return entity;
        }

        public async Task<TEntity> CreateAsync(TEntity input)
        {
            var response = await HttpClient.PostAsync(ControllerPath, GetContent(input));
            return await GetEntityAsync(response, $"POST /{ControllerPath}");
        }

        public Task<TEntity> UpdateAsync(TEntity input, params object[] id) => Implementation.UpdateAsync(input, id);
        public Task<TEntity> DeleteAsync(params object[] id) => Implementation.DeleteAsync(id);
        public Task<TEntity> FindAsync(params object[] id) => Implementation.FindRequiredAsync(id);
        public Task<TEntity> FindRequiredAsync(params object[] id) => Implementation.FindRequiredAsync(id);

        public Task<(List<TEntity> Data, int Count)> GetAsync(string where = null,
                object[] whereArgs = null, string orderBy = null, int? skip = null,
                int? take = null, CountType countType = CountType.None,
                string include = null, bool asNoTracking = true)
                => Implementation.GetAsync(where, whereArgs, orderBy, skip, take,
                    countType, include, asNoTracking);
        public Task<(List<dynamic> Data, int Count)> GetAsync(string select,
                string where = null, object[] whereArgs = null, string orderBy = null,
                int? skip = null, int? take = null, CountType countType = CountType.None,
                string include = null, bool asNoTracking = true)
                => Implementation.GetAsync(select, where, whereArgs, orderBy, skip, take,
                    countType, include, asNoTracking);

        public IEnumerable<TEntity> GetModified(DateTime asOf) => Implementation.GetModified(asOf);

        public void EnableTest(ITestOutputHelper output = null) => Implementation.EnableTest(output);
    }
}
