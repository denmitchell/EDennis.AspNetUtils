using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Xunit.Abstractions;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Controller that provides CRUD capabilities via an injected <see cref="ICrudService{TEntity}"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    [Route("api/[controller]")]
    [ApiController]
    public abstract class CrudController<TEntity> : Controller 
        where TEntity : class
    {
        /// <summary>
        /// Injected Logger
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Injected implementation of <see cref="ICrudService{TEntity}"/>
        /// </summary>
        public ICrudService<TEntity> CrudService { get; set; }


        /// <summary>
        /// Instantiates a new <see cref="CrudController{TEntity}"/> with the injected
        /// services
        /// </summary>
        /// <param name="crudService">implementation of <see cref="ICrudService{TEntity}"/></param>
        /// <param name="loggerFactory">logger factory for creating a logger</param>
        public CrudController(ICrudService<TEntity> crudService, ILoggerFactory loggerFactory)
        {
            CrudService = crudService;
            Logger = loggerFactory.CreateLogger(GetType().Name);
        }

        /// <summary>
        /// Overrideable method for resolving a string parameter to a params object[]
        /// The default implementation assumes either a single integer ID or a "/"-separated
        /// list of integer IDs that collectively represent the primary key
        /// </summary>
        /// <param name="idParam"></param>
        /// <returns></returns>
        [NonAction]
        public virtual object[] GetId(string idParam)
            => idParam.Split('/').Select(i=>(object)int.Parse(i)).ToArray();

        /// <summary>
        /// Resolves a string value to a <see cref="CountType"/> enum. 
        /// </summary>
        /// <param name="countType">the string value</param>
        /// <returns></returns>
        private static CountType GetCountType(string countType)
            => countType == null ? CountType.None : Enum.Parse<CountType>(countType);


        /// <summary>
        /// Creates a record
        /// </summary>
        /// <param name="input">the record to create</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] TEntity input)
        {
            var result = await CrudService.CreateAsync(input);
            return new ObjectResult(result) { StatusCode = 200 };
        }

        /// <summary>
        /// Updates a record
        /// </summary>
        /// <param name="input">New data for the record</param>
        /// <param name="key">The primary key of the record to update</param>
        /// <returns></returns>
        [HttpPut("{**key}")]
        public async Task<IActionResult> UpdateAsync([FromBody] TEntity input, [FromRoute] string key)
        {
            var result = await CrudService.UpdateAsync(input, GetId(key));
            if (result == null)
                return new StatusCodeResult((int)HttpStatusCode.NotFound);

            return new ObjectResult(result) { StatusCode = 200 };
        }

        /// <summary>
        /// Deletes a record
        /// </summary>
        /// <param name="key">The primary key of the record to update</param>
        /// <returns></returns>
        [HttpDelete("{**key}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string key)
        {
            var result = await CrudService.DeleteAsync(GetId(key));
            if (result == null)
                return new StatusCodeResult((int)HttpStatusCode.NotFound);

            return new ObjectResult(result) { StatusCode = 200 };
        }

        /// <summary>
        /// Gets a record
        /// </summary>
        /// <param name="key">The primary key of the record to update</param>
        /// <param name="required">whether to throw an exception if not cound</param>
        /// <returns></returns>
        [HttpGet("{**key}")]
        public async Task<IActionResult> FindAsync([FromRoute] string key)
        {
            var result = await CrudService.FindAsync(GetId(key));
            if (result == null)
                return new StatusCodeResult((int)HttpStatusCode.NotFound);

            return new ObjectResult(result) { StatusCode = 200 };
        }


        /// <summary>
        /// Uses Dynamic Linq to query records
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
        /// <returns>A Tuple with the first value being a List{TEntity} and the second value being
        /// the count across records</returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync(
                [FromQuery] string select = null,
                [FromQuery] string where = null,
                [FromQuery] object[] whereArgs = null,
                [FromQuery] string orderBy = null,
                [FromQuery] int? skip = null,
                [FromQuery] int? take = null,
                [FromQuery] string countType = null,
                [FromQuery] string include = null,
                [FromQuery] bool asNoTracking = true)
        {
            if(select == null)
            {
                (List<TEntity> Data, int Count) = await CrudService.GetAsync(where, whereArgs, orderBy, skip, take,
                        GetCountType(countType), include, asNoTracking);

                var json = JsonSerializer.Serialize((Data, Count), _jsonSerializerOptions);
                return new ContentResult { Content = json, ContentType = "application/json", StatusCode = 200 };

            } else
            {
                (List<dynamic> Data, int Count) = await CrudService.GetAsync(select, where, whereArgs, orderBy, skip, take,
                        GetCountType(countType), include, asNoTracking);

                var json = JsonSerializer.Serialize((Data, Count), _jsonSerializerOptions);
                return new ContentResult { Content = json, ContentType = "application/json", StatusCode = 200 };

            }
        }

        /// <summary>
        /// Returns all records that have been modified since the provided date.
        /// Note: this requires that the table has a PeriodStart (e.g., SysStart) column
        /// </summary>
        /// <param name="asOf">records modifed after this date are returned</param>
        /// <returns></returns>
        [HttpGet("modified")]
        public async Task<IActionResult> GetModified([FromQuery] DateTime asOf) 
        {
            var result = await CrudService.GetModifiedAsync(asOf);
            return new ObjectResult(result) { StatusCode = 200 };
        }

        /// <summary>
        /// Tells the CrudService to intiate testing mode
        /// </summary>
        /// <param name="output">A helper for outputting logs</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost("test")]
        public async Task EnableTestAsync()
        {
            await CrudService.EnableTestAsync(new ILoggerTestOutputHelper(Logger));
        }


        /// <summary>
        /// Serialization options to ensure proper serialization of tuple results
        /// </summary>
        private static JsonSerializerOptions _jsonSerializerOptions = new ()
        {
            IncludeFields = true
        };


    }

    /// <summary>
    /// Implementation of <see cref="ITestOutputHelper"/> that uses <see cref="ILogger"/>
    /// </summary>
    public class ILoggerTestOutputHelper : ITestOutputHelper
    {

        public ILogger _logger;

        public ILoggerTestOutputHelper(ILogger logger)
        {
            _logger = logger;
        }

        public void WriteLine(string message) 
            => _logger.LogDebug(message);

        public void WriteLine(string format, params object[] args) 
            => _logger.LogDebug(format, args);
    }

}
