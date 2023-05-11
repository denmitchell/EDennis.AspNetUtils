using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;

namespace EDennis.AspNetUtils
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrudService<TEntity> : Controller, ICrudService<TEntity> 
        where TEntity : class
    {

        public CrudService(ICrudService<TEntity> implementation)
        {
            Implementation = implementation;
        }

        public ICrudService<TEntity> Implementation { get; set; }

        public virtual object[] GetId(string idParam)
            => idParam.Split('/').Select(i=>(object)int.Parse(i)).ToArray();

        private static CountType GetCountType(string countType)
            => countType == null ? CountType.None : Enum.Parse<CountType>(countType);

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] TEntity input)
        {
            var result = await Implementation.CreateAsync(input);
            return new ObjectResult(result) { StatusCode = 200 };
        }

        [HttpDelete("{**key}")]
        public async Task<IActionResult> UpdateAsync([FromBody] TEntity input, [FromRoute] string key)
        {
            var result = await Implementation.UpdateAsync(input, GetId(key));
            return new ObjectResult(result) { StatusCode = 200 };
        }

        [HttpDelete("{**key}")]
        public Task<TEntity> DeleteAsync([FromRoute] string key) 
            => Implementation.DeleteAsync(GetId(key));

        [HttpGet("{**key}")]
        public Task<TEntity> FindAsync([FromRoute] string key, [FromQuery] bool required = false)
            => required ? Implementation.FindRequiredAsync(GetId(key)) : Implementation.FindAsync(GetId(key));


        [HttpGet]
        public Task<(List<TEntity> Data, int Count)> GetAsync(
                [FromQuery] string select = null,
                [FromQuery] string where = null,
                [FromQuery] object[] whereArgs = null,
                [FromQuery] string orderBy = null,
                [FromQuery] int? skip = null,
                [FromQuery] int? take = null,
                [FromQuery] string countType = null,
                [FromQuery] string include = null,
                [FromQuery] bool asNoTracking = true)
                => select == null
                    ? Implementation.GetAsync(where, whereArgs, orderBy, skip, take,
                        GetCountType(countType), include, asNoTracking)
                    : Implementation.GetAsync(select, where, whereArgs, orderBy, skip, take,
                        GetCountType(countType), include, asNoTracking);

        public IEnumerable<TEntity> GetModified(DateTime asOf) => Implementation.GetModified(asOf);

        public void EnableTest(ITestOutputHelper output = null) => Implementation.EnableTest(output);
    }
}
