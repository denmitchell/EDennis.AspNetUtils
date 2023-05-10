namespace EDennis.AspNetUtils.Tests.BlazorSample
{
    public class AppUserRolesDesignTimeDbContextFactory_AppUserContext 
        : AppUserRoleDesignTimeDbContextFactory<AppUserRolesContext>
    {
        public override IEnumerable<AppUser> AppUserData => TestRecords.GetAppUsers();
        public override IEnumerable<AppRole> AppRoleData => TestRecords.GetAppRoles();

    }
}
