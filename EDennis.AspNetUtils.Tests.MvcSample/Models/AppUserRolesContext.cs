namespace EDennis.AspNetUtils.Tests.MvcSample
{
    public class DesignTimeDbContextFactory_AppUserContext : AppUserRoleDesignTimeDbContextFactory<SimpleAuthContext>
    {
        public IEnumerable<AppRole> RoleData => TestRecords.GetAppRoles();
        public IEnumerable<AppUser> UserData => TestRecords.GetAppUsers();
    }
}
