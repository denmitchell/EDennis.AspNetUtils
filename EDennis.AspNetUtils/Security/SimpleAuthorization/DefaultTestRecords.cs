namespace EDennis.AspNetUtils
{
    public static class DefaultTestRecords
    {
        public static IEnumerable<AppRole> GetAppRoles()
        {
            var roles = new List<AppRole>
            {
                new AppRole
                {
                    Id = -1,
                    RoleName = "IT",
                    SysGuid = GuidUtils.FromId(1)
                },
                new AppRole
                {
                    Id = -2,
                    RoleName = "admin",
                    SysGuid = GuidUtils.FromId(2)
                },
                new AppRole
                {
                    Id = -3,
                    RoleName = "user",
                    SysGuid = GuidUtils.FromId(3)
                },
                new AppRole
                {
                    Id = -4,
                    RoleName = "readonly",
                    SysGuid = GuidUtils.FromId(4)
                },
                new AppRole
                {
                    Id = -5,
                    RoleName = "disabled",
                    SysGuid = GuidUtils.FromId(5)
                },
            };

            return roles;
        }

        public static IEnumerable<AppUser> GetAppUsers()
        {
            var users = new List<AppUser>
            {
                new AppUser
                {
                    Id = -1,
                    UserName = "Starbuck",
                    SysGuid = GuidUtils.FromId(1),
                    Role = "IT"
                },
                new AppUser
                {
                    Id = -2,
                    UserName = "Maria",
                    SysGuid = GuidUtils.FromId(2),
                    Role = "admin"
                },
                new AppUser
                {
                    Id = -3,
                    UserName = "Darius",
                    SysGuid = GuidUtils.FromId(3),
                    Role = "user"
                },
                new AppUser
                {
                    Id = -4,
                    UserName = "Huan",
                    SysGuid = GuidUtils.FromId(4),
                    Role = "readonly"
                },
                new AppUser
                {
                    Id = -5,
                    UserName = "Jack",
                    SysGuid = GuidUtils.FromId(5),
                    Role = "disabled"
                },
                new AppUser
                {
                    Id = -6,
                    UserName = "Watson",
                    SysGuid = GuidUtils.FromId(6),
                    Role = "IT,disabled"
                },
            };

            return users;
        }

    }
}
