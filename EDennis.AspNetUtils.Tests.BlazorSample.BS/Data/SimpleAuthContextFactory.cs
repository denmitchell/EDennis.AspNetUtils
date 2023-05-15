namespace EDennis.AspNetUtils.Tests.BlazorSample
{
    public class SimpleAuthContextFactory 
        : AppUserRoleDesignTimeDbContextFactory<SimpleAuthContext>
    {
        public override IEnumerable<AppUser> AppUserData => TestRecords.GetAppUsers();
        public override IEnumerable<AppRole> AppRoleData => TestRecords.GetAppRoles();

    }
}
