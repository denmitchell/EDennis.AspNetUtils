namespace EDennis.AspNetUtils.Tests.BlazorSample
{
    public class AppUserRolesDesignTimeDbContextFactory_AppUserContext 
        : AppUserRoleDesignTimeDbContextFactory<SimpleAuthContext>
    {
        public override IEnumerable<AppUser> AppUserData => TestRecords.GetAppUsers();
        public override IEnumerable<AppRole> AppRoleData => TestRecords.GetAppRoles();

    }
}
