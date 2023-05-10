
using Xunit.Abstractions;

namespace EDennis.AspNetUtils.Tests.BlazorSample.Tests
{
    public class AppRolesCrudServiceTestFixture :
        CrudServiceTestFixture<AppUserRolesContext, AppRoleService<AppUserRolesContext>, AppRole>
    {
    }
}
