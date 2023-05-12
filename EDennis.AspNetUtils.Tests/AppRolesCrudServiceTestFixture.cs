
namespace EDennis.AspNetUtils.Tests.BlazorSample.Tests
{
    public class AppRolesCrudServiceTestFixture :
        EntityFrameworkServiceTestFixture<SimpleAuthContext, EntityFrameworkService<SimpleAuthContext,AppRole>, AppRole>
    {
    }
}
