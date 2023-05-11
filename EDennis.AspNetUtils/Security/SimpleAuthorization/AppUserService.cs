namespace EDennis.AspNetUtils
{
    /// <summary>
    /// <see cref="EntityFrameworkService{TContext, TEntity}"/> for <see cref="AppUser"/>
    /// </summary>
    /// <typeparam name="TAppUserRolesDbContext">The relevant DbContext</typeparam>
    public class AppUserService<TAppUserRolesDbContext> : EntityFrameworkService<TAppUserRolesDbContext, AppUser>
        where TAppUserRolesDbContext : AppUserRolesContext
    {
        /// <summary>
        /// Constructs a new instance of <see cref="AppUserService{TAppUserRolesDbContext}"/>
        /// </summary>
        /// <param name="deps"></param>
        public AppUserService(CrudServiceDependencies<TAppUserRolesDbContext, AppUser> deps) : base(deps) { } 
    }
}
