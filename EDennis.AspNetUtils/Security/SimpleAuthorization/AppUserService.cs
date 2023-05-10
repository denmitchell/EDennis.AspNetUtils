namespace EDennis.AspNetUtils
{
    /// <summary>
    /// <see cref="CrudService{TContext, TEntity}"/> for <see cref="AppUser"/>
    /// </summary>
    /// <typeparam name="TAppUserRolesDbContext">The relevant DbContext</typeparam>
    public class AppUserService<TAppUserRolesDbContext> : CrudService<TAppUserRolesDbContext, AppUser>
        where TAppUserRolesDbContext : AppUserRolesContext
    {
        /// <summary>
        /// Constructs a new instance of <see cref="AppUserService{TAppUserRolesDbContext}"/>
        /// </summary>
        /// <param name="deps"></param>
        public AppUserService(CrudServiceDependencies<TAppUserRolesDbContext, AppUser> deps) : base(deps) { } 
    }
}
