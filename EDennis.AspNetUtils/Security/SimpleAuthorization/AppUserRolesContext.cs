using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EDennis.AspNetUtils
{
    /// <summary>
    /// A base DbContext for CRUD operations on <see cref="App"/>
    /// <see cref="AppUser"/> and <see cref="AppRole"/> entities.
    /// </summary>
    public partial class AppUserRolesContext : DbContext
    {

        /// <summary>
        /// The roles of the application
        /// </summary>
        public DbSet<AppRole> AppRoles { get; set; }

        /// <summary>
        /// The users of the application
        /// </summary>
        public DbSet<AppUser> AppUsers { get; set; }

        /// <summary>
        /// The name of the backing table for <see cref="AppUser" />
        /// </summary>
        public virtual string AppUserTableName { get; set; } = "AppUser";

        /// <summary>
        /// The name of the backing table for <see cref="AppRole"/>
        /// </summary>
        public virtual string AppRoleTableName { get; set; } = "AppRole";

        /// <summary>
        /// Test records for <see cref="AppRole"/>
        /// </summary>
        public virtual IEnumerable<AppRole> RoleData { get; set; }

        /// <summary>
        /// Test records for <see cref="AppUser"/>
        /// </summary>
        public virtual IEnumerable<AppUser> UserData { get; set; }

        /// <summary>
        /// Constructs a new instance of <see cref="AppUserRolesContext"/>
        /// </summary>
        /// <param name="options"></param>
        public AppUserRolesContext(DbContextOptions<AppUserRolesContext> options
            ) : base(options)
        {
        }

        /// <summary>
        /// Partial method to hook into the model building process.
        /// </summary>
        /// <param name="builder"></param>
        partial void OnModelBuilding(ModelBuilder builder);

        /// <summary>
        /// Creates the Entity Framework Model for <see cref="AppUser"/>
        /// and <see cref="AppRole"/>
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {

            OnModelBuilding(builder);


            if (Database.IsSqlServer())
            {

                builder.HasSequence<int>($"seq{AppUserTableName}",
                    opt =>
                    {
                        opt.StartsAt(1);
                        opt.IncrementsBy(1);
                    });

                builder.HasSequence<int>($"seq{AppRoleTableName}",
                    opt =>
                    {
                        opt.StartsAt(1);
                        opt.IncrementsBy(1);
                    });

                builder.Entity<AppUser>(e =>
                {
                    e.ToTable($"{AppUserTableName}", tblBuilder =>
                    {
                        tblBuilder.IsTemporal(histBuilder =>
                        {
                            histBuilder.UseHistoryTable($"{AppUserTableName}", "dbo_history");
                            histBuilder.HasPeriodStart("SysStart");
                            histBuilder.HasPeriodEnd("SysEnd");
                        });
                    });
                    e.HasKey(e => e.Id);
                    e.Property(p => p.Id)
                        .HasDefaultValueSql($@"(NEXT VALUE FOR [seq{AppUserTableName}])");


                });

                builder.Entity<AppRole>(e =>
                {
                    e.ToTable($"{AppRoleTableName}", tblBuilder =>
                    {
                        tblBuilder.IsTemporal(histBuilder =>
                        {
                            histBuilder.UseHistoryTable($"{AppRoleTableName}", "dbo_history");
                            histBuilder.HasPeriodStart("SysStart");
                            histBuilder.HasPeriodEnd("SysEnd");
                        });
                    });
                    e.HasKey(e => e.Id);
                    e.Property(p => p.Id)
                        .HasDefaultValueSql($@"(NEXT VALUE FOR [seq{AppRoleTableName}])");
                });

            }
            else
            {
                builder.Entity<AppUser>(e =>
                {
                    e.ToTable($"{AppUserTableName}");
                    e.HasKey(e => e.Id);
                    e.Property<DateTime>("SysStart")
                        .HasDefaultValue(DateTime.Now)
                        .ValueGeneratedOnAddOrUpdate();
                    e.Property<DateTime>("SysEnd")
                        .HasDefaultValue(new DateTime(9999, 12, 31, 23, 59, 59, 999));
                });

                builder.Entity<AppRole>(e =>
                {
                    e.ToTable($"{AppRoleTableName}");
                    e.HasKey(e => e.Id);
                    e.Property<DateTime>("SysStart")
                        .HasDefaultValue(DateTime.Now)
                        .ValueGeneratedOnAddOrUpdate();
                    e.Property<DateTime>("SysEnd")
                        .HasDefaultValue(new DateTime(9999, 12, 31, 23, 59, 59, 999));
                });

            }

            if (RoleData != null && RoleData.Any())
                builder.Entity<AppRole>().HasData(RoleData);

            if (UserData != null && UserData.Any())
                builder.Entity<AppUser>().HasData(UserData);


        }


    }
}