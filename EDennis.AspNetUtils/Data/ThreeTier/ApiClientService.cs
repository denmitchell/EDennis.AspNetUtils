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


        public async Task<TEntity> CreateAsync(TEntity input)
        {
            var errorMessage = $"Problem creating {typeof(TEntity).Name} instance at {ControllerPath} with {JsonSerializer.Serialize(input)}";            
            var response = await HttpClient.PostAsync(ControllerPath, GetContent(input));
            return await GetObjectAsync(response, errorMessage);
        }

        public async Task<TEntity> UpdateAsync(TEntity input, params object[] id)
        {
            var idPathParam = string.Join('/',id.Select(i=> i.ToString()));
            var errorMessage = $"Problem creating {typeof(TEntity).Name} instance at {ControllerPath}/{idPathParam} with {JsonSerializer.Serialize(input)}";
            var response = await HttpClient.PutAsync($"{ControllerPath}/{idPathParam}", GetContent(input));
            return await GetObjectAsync(response, errorMessage);
        }

        public async Task<TEntity> DeleteAsync(params object[] id)
        {
            var idPathParam = string.Join('/', id.Select(i => i.ToString()));
            var errorMessage = $"Problem deleting {typeof(TEntity).Name} instance at {ControllerPath}/{idPathParam}";
            var response = await HttpClient.DeleteAsync($"{ControllerPath}/{idPathParam}");
            return await GetObjectAsync(response, errorMessage);
        }

        public async Task<TEntity> FindAsync(params object[] id)
        {
            var idPathParam = string.Join('/', id.Select(i => i.ToString()));
            var errorMessage = $"Problem getting {typeof(TEntity).Name} instance at {ControllerPath}/{idPathParam}";
            var response = await HttpClient.GetAsync($"{ControllerPath}/{idPathParam}");
            return await GetObjectAsync(response, errorMessage);
        }

        public async Task<TEntity> FindRequiredAsync(params object[] id)
        {
            var idPathParam = string.Join('/', id.Select(i => i.ToString()));
            var errorMessage = $"Problem getting {typeof(TEntity).Name} instance at {ControllerPath}/{idPathParam}?required=true";
            var response = await HttpClient.GetAsync($"{ControllerPath}/{idPathParam}?required=true");
            return await GetObjectAsync(response, errorMessage);
        }


        public async Task<(List<TEntity> Data, int Count)> GetAsync(string where = null,
                object[] whereArgs = null, string orderBy = null, int? skip = null,
                int? take = null, CountType countType = CountType.None,
                string include = null, bool asNoTracking = true)
        {
            var queryString = QueryString(null, where, whereArgs, orderBy, skip, take, countType, include, asNoTracking);
            var errorMessage = $"Problem getting List<{typeof(TEntity).Name}> instance at {ControllerPath}/{queryString}";
            var response = await HttpClient.GetAsync($"{ControllerPath}/{queryString}");
            return await GetTupleAsync<TEntity>(response, errorMessage);
        }


        public async Task<(List<dynamic> Data, int Count)> GetAsync(string select,
                string where = null, object[] whereArgs = null, string orderBy = null,
                int? skip = null, int? take = null, CountType countType = CountType.None,
                string include = null, bool asNoTracking = true)
        {
            var queryString = QueryString(select, where, whereArgs, orderBy, skip, take, countType, include, asNoTracking);
            var errorMessage = $"Problem getting List<{typeof(TEntity).Name}> instance at {ControllerPath}/{queryString}";
            var response = await HttpClient.GetAsync($"{ControllerPath}/{queryString}");
            return await GetTupleAsync<dynamic>(response, errorMessage);
        }

        public async Task<IEnumerable<TEntity>> GetModifiedAsync(DateTime asOf)
        {
            var errorMessage = $"Problem getting modified {typeof(TEntity).Name} instances at {ControllerPath}/modified?asOf={asOf}";
            var response = await HttpClient.GetAsync($"{ControllerPath}/modified?asOf={asOf}");
            return await GetEnumerableAsync(response, errorMessage);
        }

        public async Task EnableTestAsync(ITestOutputHelper output = null)
        {
            var errorMessage = $"Problem enabling test at {ControllerPath}/test";
            var response = await HttpClient.PostAsync($"{ControllerPath}/test", null);
            if(!response.IsSuccessStatusCode)
                throw new Exception(errorMessage);
        }



        private static StringContent GetContent(TEntity entity)
            => new(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");

        private static async Task<TEntity> GetObjectAsync(HttpResponseMessage response, string exceptionMessage)
        {
            if (!response.IsSuccessStatusCode)
                throw new Exception(exceptionMessage);

            var body = await response.Content.ReadAsStringAsync();
            var entity = JsonSerializer.Deserialize<TEntity>(body);
            return entity;
        }

        private static async Task<IEnumerable<TEntity>> GetEnumerableAsync(HttpResponseMessage response, string exceptionMessage)
        {
            if (!response.IsSuccessStatusCode)
                throw new Exception(exceptionMessage);

            var body = await response.Content.ReadAsStringAsync();
            var entity = JsonSerializer.Deserialize<List<TEntity>>(body);
            return entity;
        }

        private static async Task<(List<T> Data, int Count)> GetTupleAsync<T>(HttpResponseMessage response, string exceptionMessage)
        {
            if (!response.IsSuccessStatusCode)
                throw new Exception(exceptionMessage);

            var body = await response.Content.ReadAsStringAsync();
            var entity = JsonSerializer.Deserialize<(List<T> Data, int Count)>(body, new JsonSerializerOptions { IncludeFields = true });
            return entity;
        }


        private static string QueryString(string select = null,
                string where = null, object[] whereArgs = null, string orderBy = null,
                int? skip = null, int? take = null, CountType countType = CountType.None,
                string include = null, bool asNoTracking = true)
        {
            var sb = new StringBuilder();
            if (select != null)
                sb.Append($"select={select}");
            if (where != null)
                sb.Append($"where={where}");
            if (whereArgs != null && whereArgs.Any())
                sb.Append($"whereArgs={JsonSerializer.Serialize(whereArgs)}");
            if (orderBy != null)
                sb.Append($"orderBy={orderBy}");
            if (skip != null)
                sb.Append($"skip={skip}");
            if (take != null)
                sb.Append($"take={take}");
            if (countType != CountType.None)
                sb.Append($"countType={countType.ToString()}");
            if (include != null)
                sb.Append($"include={include}");
            if (!asNoTracking)
                sb.Append($"asNoTracking={asNoTracking}");

            var str = sb.ToString();
            if (str.Length > 0)
                str = "?" + str;

            return str;
        }


    }
}
