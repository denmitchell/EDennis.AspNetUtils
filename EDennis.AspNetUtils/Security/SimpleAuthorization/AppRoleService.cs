namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Extends <see cref="CrudService{TContext, TEntity}"/> for database
    /// operations on <see cref="AppRole"/>.
    /// </summary>
    /// <typeparam name="TAppUserRolesDbContext">The DbContext used to operate on <see cref="AppRole"/></typeparam>
    public class AppRoleService<TAppUserRolesDbContext> : CrudService<TAppUserRolesDbContext, AppRole>
        where TAppUserRolesDbContext : AppUserRolesContext
    {
        /// <summary>
        /// Constructs a new instance of <see cref="AppRoleService{TAppUserRolesDbContext}"/>
        /// using the provided dependencies
        /// </summary>
        /// <param name="deps"></param>
        public AppRoleService(CrudServiceDependencies<TAppUserRolesDbContext, AppRole> deps) : base(deps) { }


    }
}
