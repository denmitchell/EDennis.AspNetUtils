using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace EDennis.AspNetUtils.Tests.BlazorSample.Tests
{
    public class CrudServiceTests_Role : IClassFixture<EntityFrameworkServiceTestFixture<SimpleAuthContext,
        EntityFrameworkService<SimpleAuthContext,AppRole>, AppRole>>
    {
        private readonly EntityFrameworkServiceTestFixture<SimpleAuthContext,
            EntityFrameworkService<SimpleAuthContext, AppRole>, 
            AppRole> _fixture;
        public const string _appsettingsFile = "appsettings.Test.json";
        private readonly ITestOutputHelper _output;


        public CrudServiceTests_Role(EntityFrameworkServiceTestFixture<SimpleAuthContext,
        EntityFrameworkService<SimpleAuthContext, AppRole>, AppRole> fixture, ITestOutputHelper output) { 
            _fixture = fixture;
            _output = output;
        }




        [Fact]
        public void TestGetAll()
        {
            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _output);

            var recs = service
                .GetQueryable()
                .Where(r => r.Id < 0) //only test roles, having negative ids
                .OrderBy(r => r.SysGuid)
                .ToList();

            XunitUtils.AssertOrderedSysGuids(recs, 1, 2, 3, 4, 5);

        }


        [Theory]
        [InlineData(1, "IT")]
        [InlineData(3, "user")]
        [InlineData(5, "disabled")]
        public void TestFind(int guidId, string roleName)
        {
            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _output);

            var guid = GuidUtils.FromId(guidId);

            var qryRec = service
                .GetQueryable()
                .FirstOrDefault(r => r.SysGuid == guid);

            Assert.NotNull(qryRec);

            var findRec = service.Find(qryRec.Id);

            Assert.Equal(roleName, findRec.RoleName);
            
        }


        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void TestDelete(int guidId)
        {
            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _output);

            //expected remaining Guids after delete
            int[] remaining = Enumerable.Range(1, 5).Except(new int[] { guidId }).ToArray();

            var guid = GuidUtils.FromId(guidId);

            var qryRec = service
                .GetQueryable()
                .FirstOrDefault(r => r.SysGuid == guid);

            Assert.NotNull(qryRec);

            service.Delete(qryRec.Id);


            var recs = service
                .GetQueryable()
                .Where(r => r.Id < 0) //only test roles, having negative ids
                .OrderBy(r => r.SysGuid)
                .ToList();

            XunitUtils.AssertOrderedSysGuids(recs, remaining);

        }


        [Theory]
        [InlineData(1, "super user")]
        [InlineData(3, "normal user")]
        public void TestUpdate(int guidId, string roleName)
        {
            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _output);

            var guid = GuidUtils.FromId(guidId);

            var qryRec = service.GetQueryable()
                .FirstOrDefault(r => r.SysGuid == guid);

            Assert.NotNull(qryRec);

            qryRec.RoleName = roleName;

            var maxSysStart = service.GetMaxSysStart();

            service.Update(qryRec, qryRec.Id);

            var recs = service
                .GetModified(maxSysStart)
                .OrderBy(r => r.SysGuid)
                .ToList();

            Assert.Collection<AppRole>(recs,
                actual =>
                {
                    Assert.Equal(roleName, actual.RoleName);
                }
            );

        }

        [Theory]
        [InlineData(998, "super user")]
        [InlineData(999, "normal user")]
        public void TestCreate(int guidId, string roleName)
        {
            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _output);

            var guid = GuidUtils.FromId(guidId);

            var AppRole = new AppRole
            {
                RoleName = roleName,
                SysGuid = guid
            };

            var maxSysStart = service.GetMaxSysStart();

            service.Create(AppRole);

            var recs = service
                .GetModified(maxSysStart)
                .OrderBy(r => r.SysGuid)
                .ToList();

            Assert.Collection<AppRole>(recs,
                actual =>
                {
                    Assert.Equal(roleName, actual.RoleName);
                    Assert.Equal(guid, actual.SysGuid);
                }
            );

        }

    }
}