using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit.Abstractions;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Implementation of <see cref="ICrudService{TEntity}"/> for ApiClients using
    /// <see cref="HttpClient"/> to communicate with an API.  Note that Exceptions 
    /// are thrown when APIs return
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ApiClientService<TEntity> : ICrudService<TEntity> where TEntity : class
    {

        /// <summary>
        /// The HttpClient for communicating with API endpoints
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// Controller path (defaults to "api/{EntityName}Controller"
        /// </summary>
        public virtual string ControllerPath { get; }

        /// <summary>
        /// Constructs a new QueryApiClient with the provided IHttpClientFactory.
        /// </summary>
        /// <param name="clientFactory">factory for creating a named HttpClient</param>
        public ApiClientService(IHttpClientFactory clientFactory)
        {
            HttpClient = clientFactory.CreateClient(typeof(TEntity).Name);
            ControllerPath ??= $"api/{typeof(TEntity).Name}Controller";
        }

        /// <summary>
        /// Creates a new record for the input
        /// </summary>
        /// <param name="input">the record to create</param>
        /// <returns></returns>
        public TEntity Create(TEntity input)
        {
            var errorMessage = $"Problem creating {typeof(TEntity).Name} instance at {ControllerPath} with {JsonSerializer.Serialize(input)}";
            var response = HttpClient.PostAsync(ControllerPath, GetContent(input)).Result;
            return GetObject(response, errorMessage);
        }

        /// <summary>
        /// Updates a record
        /// </summary>
        /// <param name="input">Data to use for the update</param>
        /// <param name="id">The primary key of the record to update</param>
        /// <returns></returns>
        public TEntity Update(TEntity input, params object[] id)
        {
            var idPathParam = string.Join('/', id.Select(i => i.ToString()));
            var errorMessage = $"Problem creating {typeof(TEntity).Name} instance at {ControllerPath}/{idPathParam} with {JsonSerializer.Serialize(input)}";
            var response = HttpClient.PutAsync($"{ControllerPath}/{idPathParam}", GetContent(input)).Result;
            return GetObject(response, errorMessage);
        }

        /// <summary>
        /// Deletes a record
        /// </summary>
        /// <param name="id">The primary key of the record to update</param>
        /// <returns></returns>
        public TEntity Delete(params object[] id)
        {
            var idPathParam = string.Join('/', id.Select(i => i.ToString()));
            var errorMessage = $"Problem deleting {typeof(TEntity).Name} instance at {ControllerPath}/{idPathParam}";
            var response = HttpClient.DeleteAsync($"{ControllerPath}/{idPathParam}").Result;
            return GetObject(response, errorMessage);
        }

        /// <summary>
        /// Gets a record
        /// </summary>
        /// <param name="id">The primary key of the record to get</param>
        /// <returns></returns>
        public TEntity Find(params object[] id)
        {
            var idPathParam = string.Join('/', id.Select(i => i.ToString()));
            var errorMessage = $"Problem getting {typeof(TEntity).Name} instance at {ControllerPath}/{idPathParam}";
            var response = HttpClient.GetAsync($"{ControllerPath}/{idPathParam}").Result;
            return GetObject(response, errorMessage);
        }


        /// <summary>
        /// Uses Dynamic Linq to query records
        /// </summary>
        /// <param name="where">the Where (filter) clause</param>
        /// <param name="whereArgs">when the Where clause has placeholders, this resolves the placeholder to values</param>
        /// <param name="orderBy">the OrderBy (sort) clause</param>
        /// <param name="skip">how many records to skip (needed for paging)</param>
        /// <param name="take">how many records to take (needed for paging)</param>
        /// <param name="countType">the type of record counting across all pages</param>
        /// <param name="include">where navigation properties to include</param>
        /// <param name="asNoTracking">whether to not track the entity</param>
        /// <returns>A Tuple with the first value being a List{TEntity} and the second value being
        /// the count across records</returns>
        public (List<TEntity> Data, int Count) Get(string where = null,
                object[] whereArgs = null, string orderBy = null, int? skip = null,
                int? take = null, CountType countType = CountType.None,
                string include = null, bool asNoTracking = true)
        {
            var queryString = QueryString(null, where, whereArgs, orderBy, skip, take, countType, include, asNoTracking);
            var errorMessage = $"Problem getting List<{typeof(TEntity).Name}> instance at {ControllerPath}/{queryString}";
            var response = HttpClient.GetAsync($"{ControllerPath}/{queryString}").Result;
            return GetTuple<TEntity>(response, errorMessage);
        }

        /// <summary>
        /// Uses Dynamic Linq to query records.  This method returns a dynamic list.
        /// </summary>
        /// <param name="select">the Select clause (which properties/columns to include)</param>
        /// <param name="where">the Where (filter) clause</param>
        /// <param name="whereArgs">when the Where clause has placeholders, this resolves the placeholder to values</param>
        /// <param name="orderBy">the OrderBy (sort) clause</param>
        /// <param name="skip">how many records to skip (needed for paging)</param>
        /// <param name="take">how many records to take (needed for paging)</param>
        /// <param name="countType">the type of record counting across all pages</param>
        /// <param name="include">where navigation properties to include</param>
        /// <param name="asNoTracking">whether to not track the entity</param>
        /// <returns>A Tuple with the first value being a List{dynamic} and the second value being
        /// the count across records</returns>
        public (List<dynamic> Data, int Count) Get(string select,
                string where = null, object[] whereArgs = null, string orderBy = null,
                int? skip = null, int? take = null, CountType countType = CountType.None,
                string include = null, bool asNoTracking = true)
        {
            var queryString = QueryString(select, where, whereArgs, orderBy, skip, take, countType, include, asNoTracking);
            var errorMessage = $"Problem getting List<{typeof(TEntity).Name}> instance at {ControllerPath}/{queryString}";
            var response = HttpClient.GetAsync($"{ControllerPath}/{queryString}").Result;
            return GetTuple<dynamic>(response, errorMessage);
        }

        /// <summary>
        /// Returns all records that have been modified since the provided date.
        /// Note: this requires that the table has a PeriodStart (e.g., SysStart) column
        /// </summary>
        /// <param name="asOf">records modifed after this date are returned</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetModified(DateTime asOf)
        {
            var errorMessage = $"Problem getting modified {typeof(TEntity).Name} instances at {ControllerPath}/modified?asOf={asOf}";
            var response = HttpClient.GetAsync($"{ControllerPath}/modified?asOf={asOf}").Result;
            return GetEnumerable(response, errorMessage);
        }

        /// <summary>
        /// Tells the CrudService to intiate testing mode
        /// </summary>
        /// <param name="output">A helper for outputting logs</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public void EnableTest(ITestOutputHelper output = null)
        {
            var errorMessage = $"Problem enabling test at {ControllerPath}/test";
            var response = HttpClient.PostAsync($"{ControllerPath}/test", null).Result;
            if (!response.IsSuccessStatusCode)
                throw new Exception(errorMessage);
        }


        /// <summary>
        /// Produces an <see cref="HttpContext"/> object, suitable as an HTTP Request body
        /// </summary>
        /// <param name="entity">the object to send via HTTP request</param>
        /// <returns></returns>
        private static StringContent GetContent(TEntity entity)
            => new(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");

        /// <summary>
        /// Extracts an object from an <see cref="HttpResponseMessage"/> body.
        /// </summary>
        /// <param name="response">The response message</param>
        /// <param name="exceptionMessage">An exception message to use if the status code 
        /// represents an error</param>
        /// <param name="ignoreStatusCodes">Status codes to ignore (not throw exceptions for)</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static TEntity GetObject(HttpResponseMessage response, string exceptionMessage)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            else if (!response.IsSuccessStatusCode)
                throw new Exception(exceptionMessage);

            var body = response.Content.ReadAsStringAsync().Result;
            var entity = JsonSerializer.Deserialize<TEntity>(body);
            return entity;
        }

        /// <summary>
        /// Extracts an IEnumerable from an <see cref="HttpResponseMessage"/> body.
        /// </summary>
        /// <param name="response">The response message</param>
        /// <param name="exceptionMessage">An exception message to use if the status code 
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static IEnumerable<TEntity> GetEnumerable(HttpResponseMessage response, string exceptionMessage)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            else if (!response.IsSuccessStatusCode)
                throw new Exception(exceptionMessage);

            var body = response.Content.ReadAsStringAsync().Result;
            var entity = JsonSerializer.Deserialize<List<TEntity>>(body);
            return entity;
        }

        /// <summary>
        /// Extracts a tuple from an <see cref="HttpResponseMessage"/> body.
        /// </summary>
        /// <typeparam name="T">The record type</typeparam>
        /// <param name="response">The response message</param>
        /// <param name="exceptionMessage">An exception message to use if the status code 
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static (List<T> Data, int Count) GetTuple<T>(HttpResponseMessage response, string exceptionMessage)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                return default;
            else if (!response.IsSuccessStatusCode)
                throw new Exception(exceptionMessage);

            var body = response.Content.ReadAsStringAsync().Result;
            var entity = JsonSerializer.Deserialize<(List<T> Data, int Count)>(body, new JsonSerializerOptions { IncludeFields = true });
            return entity;
        }

        /// <summary>
        /// Builds a query string from Dynamic Linq parameters
        /// </summary>
        /// <param name="select">the Select clause (which properties/columns to include)</param>
        /// <param name="where">the Where (filter) clause</param>
        /// <param name="whereArgs">when the Where clause has placeholders, this resolves the placeholder to values</param>
        /// <param name="orderBy">the OrderBy (sort) clause</param>
        /// <param name="skip">how many records to skip (needed for paging)</param>
        /// <param name="take">how many records to take (needed for paging)</param>
        /// <param name="countType">the type of record counting across all pages</param>
        /// <param name="include">where navigation properties to include</param>
        /// <param name="asNoTracking">whether to not track the entity</param>
        /// <returns></returns>
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
