using EDennis.AspNetUtils.Tests.BlazorSample.Shared.Models;
using Xunit.Abstractions;

namespace EDennis.AspNetUtils.Tests.BlazorSample.Tests
{
    public class CrudServiceTests_Song : IClassFixture<EntityFrameworkServiceTestFixture<HitsContext,
        EntityFrameworkService<HitsContext,Song>, Song>>
    {
        private readonly EntityFrameworkServiceTestFixture<HitsContext, EntityFrameworkService<HitsContext, Song>, Song> _fixture;
        public const string _appsettingsFile = "appsettings.Test.json";
        private readonly ITestOutputHelper _output;


        public CrudServiceTests_Song(EntityFrameworkServiceTestFixture<HitsContext,
        EntityFrameworkService<HitsContext, Song>, Song> fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }


        private static Dictionary<string, object[]> _filterArgs = new()
        {
            {"TestGetPage_Date", new object[] { new DateTime(1970,1,1) } },
            {"TestGetPage_Title", new object[] { "o" } }
        };

        [Theory]
        [InlineData("ReleaseDate > @0", "TestGetPage_Date", "Title", 2, 3, 11, /*guidIds:*/ 6, 7, 5)]
        [InlineData("Title.Contains(@0)", "TestGetPage_Title", "ReleaseDate desc", 3, 4, 9, /*guidIds:*/ 11, 16, 3, 15 )]
        public void TestGetPageNoSelect(string where, string whereArgsKey, string orderBy, int skip, int take, 
            int countAcrossPages, params int[] guidIds)
        {
            var whereArgs = whereArgsKey == null ? null : _filterArgs[whereArgsKey];

            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _output);

            var (data, count) = service
                .Get(where, whereArgs,orderBy,skip,take,countType:CountType.Count);

                Assert.Equal(countAcrossPages, count);
                XunitUtils.AssertOrderedSysGuids(data, guidIds);            

        }

        [Theory]
        [InlineData("ReleaseDate > \"1970-01-01\"", "Title", 2, 3, 11, /*guidIds:*/ 6, 7, 5)]
        [InlineData("Title.Contains(\"o\")", "ReleaseDate desc", 3, 4, 9, /*guidIds:*/ 11, 16, 3, 15)]
        public void TestGetDynamicLinqNoSelect(string where, string orderBy, int skip, int take, int countAcrossPages, params int[] guidIds)
        {
            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _output);

            var (data, count) = service
                .Get(where,null,orderBy,skip,take, countType: CountType.Count);

            Assert.Equal(countAcrossPages, count);
            XunitUtils.AssertOrderedSysGuids(data, guidIds);

        }

        public class SysGuidTitle { public Guid SysGuid { get; set; } public string Title { get; set; } }
        public class SysGuidReleaseDate { public Guid SysGuid { get; set; } public DateTime ReleaseDate { get; set; } }

        private static readonly Dictionary<string, (Guid SysGuid, dynamic Prop)[]> _select = new()
        {
            {
                "new {SysGuid, Title}", new (Guid, dynamic)[] {
                    (GuidUtils.FromId(6), "Fool in the Rain"), 
                    (GuidUtils.FromId(7), "Hotel California"),
                    (GuidUtils.FromId(5), "Kashmir"),
                }
            },
            {
                "new {SysGuid, ReleaseDate}", new (Guid, dynamic)[] {
                    (GuidUtils.FromId(11), new DateTime(1975, 10, 13)),
                    (GuidUtils.FromId(16), new DateTime(1975, 5, 19)),
                    (GuidUtils.FromId(3), new DateTime(1973, 11, 2)),
                    (GuidUtils.FromId(15), new DateTime(1973, 6, 27)),
                }
            }
        };


        [Theory]
        [InlineData("new {SysGuid, Title}", "ReleaseDate > @0", "TestGetPage_Date", "Title", 2, 3, 11)]
        [InlineData("new {SysGuid, ReleaseDate}", "Title.Contains(@0)", "TestGetPage_Title", "ReleaseDate desc", 3, 4, 9)]
        public void TestGetPageSelect(string select, string where, string whereArgsKey, string orderBy, int skip, int take, int countAcrossPages)
        {
            var whereArgs = whereArgsKey == null ? null : _filterArgs[whereArgsKey];
            var expected = _select[select];

            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _output);

            var (data, count) = service
                .Get(select,where,whereArgs,orderBy,skip,take, countType: CountType.Count);

            Assert.Equal(countAcrossPages, count);

            var range = Enumerable.Range(0, Math.Min(expected.Length, count)).ToList();

            if (select == "new {SysGuid, Title}")
            {
                var typedResults = TypeUtils.Cast<List<object>, List<SysGuidTitle>>(data);

                IEnumerable<Action<SysGuidTitle>> asserts =
                    range.Select<int,Action<SysGuidTitle>>(i =>
                    rec =>
                    {
                        Assert.Equal(expected[i].SysGuid, rec.SysGuid);
                        Assert.Equal(expected[i].Prop, rec.Title);
                    });

                Assert.Collection(typedResults, asserts.ToArray());
            }
            else if (select == "new {SysGuid, ReleaseDate}")
            {
                var typedResults = TypeUtils.Cast<List<object>, List<SysGuidReleaseDate>>(data);

                IEnumerable<Action<SysGuidReleaseDate>> asserts =
                    range.Select<int, Action<SysGuidReleaseDate>>(i =>
                    rec =>
                    {
                        Assert.Equal(expected[i].SysGuid, rec.SysGuid);
                        Assert.Equal(expected[i].Prop, rec.ReleaseDate);
                    });

                Assert.Collection(typedResults, asserts.ToArray());
            }
        }

        [Theory]
        [InlineData("new {SysGuid, Title}", "ReleaseDate > \"1970-01-01\"", "Title", 2, 3, 11)]
        [InlineData("new {SysGuid, ReleaseDate}", "Title.Contains(\"o\")", "ReleaseDate desc", 3, 4, 9)]
        public void TestGetDynamicLinqSelect(string select, string where, string orderBy, int skip, int take, int countAcrossPages)
        {
            var expected = _select[select];

            var service = _fixture.GetCrudService(_appsettingsFile, "Starbuck", _output);

            var (data, count) = service
                .Get(select, where, null, orderBy, skip, take, countType: CountType.Count);

            Assert.Equal(countAcrossPages, count);

            var range = Enumerable.Range(0, Math.Min(expected.Length, count)).ToList();

            if (select == "new {SysGuid, Title}")
            {
                var typedResults = TypeUtils.Cast<List<object>, List<SysGuidTitle>>(data);

                IEnumerable<Action<SysGuidTitle>> asserts =
                    range.Select<int, Action<SysGuidTitle>>(i =>
                    rec =>
                    {
                        Assert.Equal(expected[i].SysGuid, rec.SysGuid);
                        Assert.Equal(expected[i].Prop, rec.Title);
                    });

                Assert.Collection(typedResults, asserts.ToArray());
            }
            else if (select == "new {SysGuid, ReleaseDate}")
            {
                var typedResults = TypeUtils.Cast<List<object>, List<SysGuidReleaseDate>>(data);

                IEnumerable<Action<SysGuidReleaseDate>> asserts =
                    range.Select<int, Action<SysGuidReleaseDate>>(i =>
                    rec =>
                    {
                        Assert.Equal(expected[i].SysGuid, rec.SysGuid);
                        Assert.Equal(expected[i].Prop, rec.ReleaseDate);
                    });

                Assert.Collection(typedResults, asserts.ToArray());
            }
        }

    }
}