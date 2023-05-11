using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace EDennis.AspNetUtils
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrudController<TEntity> : Controller 
        where TEntity : class
    {

        ILogger Logger { get; }

        public CrudController(ICrudService<TEntity> crudService, ILoggerFactory loggerFactory)
        {
            CrudService = crudService;
            Logger = loggerFactory.CreateLogger(GetType().Name);
        }

        public ICrudService<TEntity> CrudService { get; set; }

        public virtual object[] GetId(string idParam)
            => idParam.Split('/').Select(i=>(object)int.Parse(i)).ToArray();

        private static CountType GetCountType(string countType)
            => countType == null ? CountType.None : Enum.Parse<CountType>(countType);

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] TEntity input)
        {
            var result = await CrudService.CreateAsync(input);
            return new ObjectResult(result) { StatusCode = 200 };
        }

        [HttpPut("{**key}")]
        public async Task<IActionResult> UpdateAsync([FromBody] TEntity input, [FromRoute] string key)
        {
            var result = await CrudService.UpdateAsync(input, GetId(key));
            return new ObjectResult(result) { StatusCode = 200 };
        }

        [HttpDelete("{**key}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string key)
        {
            var result = await CrudService.DeleteAsync(GetId(key));
            return new ObjectResult(result) { StatusCode = 200 };
        }


        [HttpGet("{**key}")]
        public async Task<IActionResult> FindAsync([FromRoute] string key, [FromQuery] bool required = false)
        {
            var result = required
                ? await CrudService.FindRequiredAsync(GetId(key))
                : await CrudService.FindAsync(GetId(key));

            return new ObjectResult(result) { StatusCode = 200 };
        }


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
                (List<TEntity> data, int count) = await CrudService.GetAsync(where, whereArgs, orderBy, skip, take,
                        GetCountType(countType), include, asNoTracking);
                return new ObjectResult((data,count)) { StatusCode = 200 };
            } else
            {
                (List<dynamic> data, int count) = await CrudService.GetAsync(select, where, whereArgs, orderBy, skip, take,
                        GetCountType(countType), include, asNoTracking);
                return new ObjectResult((data, count)) { StatusCode = 200 };

            }
        }

        [HttpGet("modified")]
        public async Task<IActionResult> GetModified([FromQuery] DateTime asOf) 
        {
            var result = await CrudService.GetModifiedAsync(asOf);
            return new ObjectResult(result) { StatusCode = 200 };
        }

        [HttpPost("test")]
        public async Task EnableTestAsync()
        {
            await CrudService.EnableTestAsync(new ILoggerTestOutputHelper(Logger));
        }
    }


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
