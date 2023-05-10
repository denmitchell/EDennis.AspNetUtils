using EDennis.AspNetUtils.Tests.MvcSample;
using Microsoft.EntityFrameworkCore;

namespace EDennis.AspNetUtils.Tests.MvcSample
{
    public class DesignTimeDbContextFactory_AppUserContext : AppUserRoleDesignTimeDbContextFactory<AppUserRolesContext>
    {
        public IEnumerable<AppRole> RoleData => TestRecords.GetAppRoles();
        public IEnumerable<AppUser> UserData => TestRecords.GetAppUsers();
    }
}
