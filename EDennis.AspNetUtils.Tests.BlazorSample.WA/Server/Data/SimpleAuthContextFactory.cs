namespace EDennis.AspNetUtils.Tests.BlazorSample.WA.Server
{
    public class SimpleAuthContextFactory 
        : AppUserRoleDesignTimeDbContextFactory<SimpleAuthContext>
    {
        public override IEnumerable<AppUser> AppUserData => TestRecords.GetAppUsers();
        public override IEnumerable<AppRole> AppRoleData => TestRecords.GetAppRoles();

    }
}
