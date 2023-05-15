using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Xunit.Abstractions;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// Generic repository service for performing CRUD operations against
    /// a database using Entity Framework.  The service provides some 
    /// helpful features, including:
    /// <list type="bullet">
    /// <item>Automatic updates to SysUser for all tracked changed entities,
    /// including entities that are not of type <see cref="TEntity"/>.</item>
    /// </list>
    /// <item>Support for setting a transaction that can automatically rollback
    /// after an automated test or interactive/manual testing 
    /// </item>
    /// <item>Support for Dynamic Linq and Radzen Queries (which use Dynamic 
    /// Linq behind the scenes)</item>
    /// </summary>
    /// <typeparam name="TContext">The DbContext class</typeparam>
    /// <typeparam name="TEntity">The model/entity class</typeparam>
    public class EntityFrameworkService<TContext, TEntity> : ICrudService<TEntity> where TContext : DbContext
        where TEntity : class
    {
        #region Variables

        /// <summary>
        /// Allows specifying a different SysStart (PeriodStart) column for
        /// SQL Server temporal tables.
        /// </summary>
        public virtual string SysStartColumn { get; } = "SysStart";

        /// <summary>
        /// The Entity Framework DbContext class for communicating with the
        /// database.
        /// </summary>
        public TContext DbContext { get; private set; }

        /// <summary>
        /// The name of the authenticated UserName Principal, which is 
        /// used to update SysUser
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// A service for replacing the normal DbContext with a DbContext
        /// that is more suitable for testing (e.g., one with an open
        /// transaction).
        /// </summary>
        private readonly DbContextService<TContext> _dbContextService;

        /// <summary>
        /// A cache that is used to hold the countType of records across pages
        /// for specific queries (by where/filter clause).  This is helpful
        /// to prevent recounting records when the user is merely paging
        /// across the same logical result set.
        /// </summary>
        private readonly CountCache<TEntity> _countCache;

        /// <summary>
        /// The table name associated with the entity.
        /// </summary>
        public static string TableName { get; private set; }

        /// <summary>
        /// Overrideable threshold for expiring the cached countType of records.  This value
        /// need not be large because it is mainly supporting paging across records, which
        /// typically is done fairly quickly by users.
        /// </summary>
        public virtual double CountCacheExpirationInSeconds { get; private set; } = 60;

        /// <summary>
        /// Gets the table name associated with the entity
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            if (TableName == null)
            {
                var entityType = typeof(TEntity);
                var modelEntityType = DbContext.Model.FindEntityType(entityType);
                TableName = modelEntityType.GetSchemaQualifiedTableName();
            }
            return TableName;
        }


        #endregion
        #region Constructor

        /// <summary>
        /// Constructs a new instance of <see cref="EntityFrameworkService{TContext, TEntity}"/> with
        /// various <see cref="EntityFrameworkServiceDependencies{TContext, TEntity}"/> injected.
        /// The constructor sets up a reference to the DbContext, the DbContextService
        /// (for replacing the DbContext during testing), and the <see cref="CountCache{TEntity}"/>.
        /// The constructor also uses the <see cref="MvcAuthenticationStateProvider"/> to
        /// set the UserName property from the UserName Principal name.
        /// </summary>
        /// <param name="deps">A bundled set of dependencies to inject</param>
        public EntityFrameworkService(EntityFrameworkServiceDependencies<TContext, TEntity> deps)
        {
            _dbContextService = deps.DbContextService;
            _countCache = deps.CountCache;

            DbContext = deps.DbContextService.GetDbContext(deps.Configuration);
            SetUserName(deps.UserNameProvider);
        }

        #endregion
        #region Testing Support

        /// <summary>
        /// Uses the DbContextService to replace the existing DbContext with a 
        /// context that is more suitable for testing (e.g., one having an open
        /// transaction that automatically rolls back)
        /// </summary>
        /// <param name="testDbContextType">The nature of the new DbContext</param>
        /// <param name="output">An Xunit helper class for piping logs to the appropriate
        /// output stream during testing</param>
        public void EnableTest(ITestOutputHelper output = null)
        {
            DbContext = _dbContextService.GetTestServiceContext(output);
        }


        #endregion
        #region Write Operations

        /// <summary>
        /// Sets the UserName property of this service based upon the UserName Principal's 
        /// Name claim.  This class uses <see cref="MvcAuthenticationStateProvider"/>
        /// </summary>
        /// <param name="authorizationProvider"></param>
        public void SetUserName(UserNameProvider userNameProvider)
        {
            UserName = userNameProvider.UserName;
        }

        /// <summary>
        /// Inserts a new record into the database, based upon data in
        /// the provided entity.
        /// </summary>
        /// <param name="input">The entity holding data to insert into the database</param>
        /// <returns></returns>
        public virtual TEntity Create(TEntity input)
        {
            UpdateSysUser();
            BeforeCreate(input); //optional lifecycle method

            //update SysGuid when relevant
            if (input is IHasSysGuid iHasSysGuid && iHasSysGuid.SysGuid == default)
                iHasSysGuid.SysGuid = Guid.NewGuid();

            DbContext.Add(input);
            DbContext.SaveChanges();

            AfterCreate(input); //optional lifecycle method

            return input;
        }


        /// <summary>
        /// Updates a record in the database, based upon data
        /// contained in the provided entity.  Note that the 
        /// Id should match the primary key of the provided entity.
        /// </summary>
        /// <param name="input">The entity holding data to update</param>
        /// <param name="id">The primary key of the entity</param>
        /// <returns></returns>
        public virtual TEntity Update(
            TEntity input, params object[] id)
        {
            var existing = Find(id);
            if (existing == null)
                return null;

            BeforeUpdate(existing); //optional lifecycle method

            //Entity Framework has some special methods for
            //updating entities.  This is the recommended pattern:
            var entry = DbContext.Entry(existing);
            entry.CurrentValues.SetValues(input);
            entry.State = EntityState.Modified;

            UpdateSysUser();
            DbContext.SaveChanges();

            AfterUpdate(existing); //optional lifecycle method

            return input;

        }



        /// <summary>
        /// Deletes an existing entity
        /// </summary>
        /// <param name="key">the primary key of the entity</param>
        /// <returns>OK or NoContent, if successful</returns>
        /// <seealso cref="Delete(string)"/>
        public virtual TEntity Delete(params object[] id)
        {
            var existing = Find(id);

            if (existing == null)
                return null;

            BeforeDelete(existing);

            UpdateSysUser();
            DbContext.SaveChanges();
            DbContext.Remove(existing);
            DbContext.SaveChanges();

            AfterDelete(existing);

            return existing;
        }

        /// <summary>
        /// Updates SysUser in all changed entities that implement IHasSysUser
        /// </summary>
        public virtual void UpdateSysUser()
        {
            var entries = DbContext.ChangeTracker.Entries().ToList();
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry.Entity is IHasSysUser entity)
                    switch (entry.State)
                    {
                        case EntityState.Added:
                        case EntityState.Modified:
                        case EntityState.Deleted:
                            entity.SysUser = UserName;
                            break;
                        default:
                            break;
                    }
            }
        }

        #endregion
        #region Lifecycle Methods

        /// <summary>
        /// Overrideable method that will be executed before Create
        /// </summary>
        /// <param name="input">The entity to create</param>
        public virtual void BeforeCreate(TEntity input) { }

        /// <summary>
        /// Overrideable method that will be executed after Create
        /// </summary>
        /// <param name="input">The entity to create</param>
        public virtual void AfterCreate(TEntity input) { }

        /// <summary>
        /// Overrideable method that will be executed before Update
        /// </summary>
        /// <param name="existing">The entity to update</param>
        public virtual void BeforeUpdate(TEntity existing) { }

        /// <summary>
        /// Overrideable method that will be executed after Update
        /// </summary>
        /// <param name="existing">The entity to update</param>
        public virtual void AfterUpdate(TEntity existing) { }

        /// <summary>
        /// Overrideable method that will be executed before Delete
        /// </summary>
        /// <param name="existing">The entity to delete</param>
        public virtual void BeforeDelete(TEntity existing) { }

        /// <summary>
        /// Overrideable method that will be executed after Delete
        /// </summary>
        /// <param name="existing">The entity to delete</param>
        public virtual void AfterDelete(TEntity existing) { }

        #endregion
        #region Basic Read Operations

        /// <summary>
        /// Finds a record based upon the provided primary key.  Note that
        /// this method will return null if the record isn't found.
        /// See also <see cref="FindAsync(object[])"/>
        /// </summary>
        /// <param name="id">The primary key of the target record</param>
        /// <returns></returns>
        public TEntity Find(params object[] id)
            => DbContext.Find<TEntity>(id);



        /// <summary>
        /// Merely returns a typed IQueryable.  DevExpress supports binding
        /// IQueryables to their data grid.
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetQueryable(bool asNoTracking = true)
        {
            var dbSet = DbContext
                .Set<TEntity>();

            IQueryable<TEntity> qry = dbSet.AsQueryable();

            if (asNoTracking)
                qry = qry
                .AsNoTracking();

            return qry;
        }

        #endregion
        #region Dynamic Linq

        /// <summary>
        /// Asynchronously gets a dynamic list result using a Dynamic Linq Expression
        /// https://github.com/StefH/System.Linq.Dynamic.Core
        /// https://github.com/StefH/System.Linq.Dynamic.Core/wiki/Dynamic-Expressions
        /// </summary>
        /// <param name="select">Select expression (which columns to include)</param>
        /// <param name="where">string Where expression (how to filter the resultset)</param>
        /// <param name="whereArgs">when the where argument includes placeholders, the whereArgs resolves
        /// those placeholders to values</param>
        /// <param name="orderBy">string OrderBy expression (with support for descending)</param>
        /// <param name="skip">int number of records to skip</param>
        /// <param name="take">int number of records to return</param>
        /// <param name="countType">the <see cref="CountType"/> for the query</param>
        /// <param name="include">string Include expression (for including navigation properties)</param>
        /// <param name="asNoTracking">whether to track entities (for updating)</param>
        /// <returns>Dynamic-typed object</returns>
        /// <seealso cref="Get(string, object[], string, int?, int?, CountType, string, bool)"/>
        public virtual (List<dynamic> Data, int Count) Get(
                string select,
                string where = null, object[] whereArgs = null,
                string orderBy = null, int? skip = null, int? take = null,
                CountType countType = CountType.None, string include = null,
                bool asNoTracking = true
                )
        {

            var (query, recCount) = BuildQuery(where, whereArgs, orderBy, skip, take, countType, include, asNoTracking);

            List<dynamic> data = null;
            if (countType != CountType.CountOnly)
                data = query.Select(select)
                    .ToDynamicList();

            return (data, recCount);
        }


        /// <summary>
        /// Asynchronously gets an entity-typed list result using a Dynamic Linq Expression
        /// https://github.com/StefH/System.Linq.Dynamic.Core
        /// https://github.com/StefH/System.Linq.Dynamic.Core/wiki/Dynamic-Expressions
        /// </summary>
        /// <param name="where">string Where expression (how to filter the resultset)</param>
        /// <param name="whereArgs">when the where argument includes placeholders, the whereArgs resolves
        /// those placeholders to values</param>
        /// <param name="orderBy">string OrderBy expression (with support for descending)</param>
        /// <param name="skip">int number of records to skip</param>
        /// <param name="take">int number of records to return</param>
        /// <param name="countType">the <see cref="CountType"/> for the query</param>
        /// <param name="include">string Include expression (for including navigation properties)</param>
        /// <param name="asNoTracking">whether to track entities (for updating)</param>
        /// <returns>Entity-typed object</returns>
        /// <seealso cref="Get(string, string, object[], string, int?, int?, CountType, string, bool)"/>
        public virtual (List<TEntity> Data, int Count) Get(
                string where = null, object[] whereArgs = null,
                string orderBy = null, int? skip = null, int? take = null,
                CountType countType = CountType.None, string include = null,
                bool asNoTracking = true
                )
        {

            var (query, recCount) = BuildQuery(where, whereArgs, orderBy, skip, take, countType, include, asNoTracking);

            List<TEntity> data = null;
            if (countType != CountType.CountOnly)
                data = query.ToList();

            return (data, recCount);

        }


        #endregion
        #region Helper Methods


        /// <summary>
        /// Gets any modified records (helpful for testing purposes)
        /// </summary>
        /// <param name="asOf">Get all modifications after this datetime.
        /// NOTE: do not make this >=.  It will not work!</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetModified(DateTime asOf)
        {
            var results = DbContext.Set<TEntity>()
                .Where(e => EF.Property<DateTime>(e, SysStartColumn) > asOf)
                .ToList();

            return results;
        }

        /// <summary>
        /// Gets the most recent create/update datetime value for a given entity.
        /// When retrieved before an operation is performed, it can be used in 
        /// combination with <see cref="GetModified(DateTime)"/> to obtain all
        /// entities that were modified as a result of the operation (only when 
        /// isolated testing is performed).
        /// </summary>
        /// <returns></returns>
        public DateTime GetMaxSysStart()
        {
            FormattedString sql = new($"SELECT MAX({SysStartColumn}) Value FROM {GetTableName()}");

            return DbContext.Database
                .SqlQuery<DateTime>(sql)
                .FirstOrDefault();
        }


        /// <summary>
        /// Applies Dynamic Linq extension methods to build the queryable
        /// </summary>
        /// <param name="include">Navigation properties to include</param>
        /// <param name="where">where clause (Where extension method)</param>
        /// <param name="whereArgs">where arguments (when where clause has placeholders)</param>
        /// <param name="orderBy">order by clause (OrderBy extension method)</param>
        /// <param name="skip">records to skip</param>
        /// <param name="take">records to take</param>
        /// <param name="countType">whether to countType the records across pages</param>
        /// <param name="asNoTracking">whether to track entities (for updates)</param>
        /// <returns></returns>
        private (IQueryable<TEntity> Query, int Count) BuildQuery(
                string where = null, object[] whereArgs = null, string orderBy = null,
                int? skip = null, int? take = null, CountType countType = CountType.None,
                string include = null, bool asNoTracking = true)
        {

            int recCount = -1;

            var dbSet = DbContext
            .Set<TEntity>();

            IQueryable<TEntity> query = dbSet.AsQueryable();


            //handle Expand/Include -- including child records from navigation properties
            if (!string.IsNullOrWhiteSpace(include))
            {
                var includes = include.Split(',');
                foreach (var incl in includes)
                    query = query.Include(incl);
            }

            //handle Filter/Where -- limited the result set by a condition
            if (!string.IsNullOrEmpty(where))
            {
                if (whereArgs != null)
                    query = query.Where(where, whereArgs);
                else
                    query = query.Where(where);
            }

            if (countType != CountType.None)
                //get the countType of records.  Note that the countType of records
                recCount = _countCache.GetCount(query, where, whereArgs, TimeSpan.FromSeconds(CountCacheExpirationInSeconds));

            if (countType != CountType.CountOnly)
            {
                //handle OrderBy -- server-side sorting of records
                if (!string.IsNullOrEmpty(orderBy))
                {
                    query = query.OrderBy(orderBy);
                }

                //handle skipping of a certain number of records -- part of paging
                if (skip != null && skip > 0)
                {
                    query = query.Skip(skip.Value);
                }

                //handling taking of a certain number of records -- part of paging
                if (take != null && take > 0)
                {
                    query = query.Take(take.Value);
                }

                if (asNoTracking)
                {
                    query = query.AsNoTracking();
                }
            }

            //return the query and the countType across records
            return (query, recCount);
        }

        #endregion

    }

    /// <summary>
    /// Workaround class to support dynamically building SQL string for 
    /// <see cref="EntityFrameworkService{TContext, TEntity}.GetMaxSysStart"/>
    /// </summary>
    public class FormattedString : FormattableString
    {
        private readonly string _str;
        public FormattedString(FormattableString str)
        {
            _str = str.ToString();
        }

        public override int ArgumentCount => 0;

        public override string Format => _str;

        public override object GetArgument(int index)
        {
            return null;
        }

        public override object[] GetArguments()
        {
            return Array.Empty<object>();
        }

        public override string ToString(IFormatProvider formatProvider)
        {
            return _str;
        }
    }
}
