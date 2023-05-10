﻿// <auto-generated />
using System;
using EDennis.AspNetUtils.Tests.MvcSample;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EDennis.AspNetUtils.Tests.MvcSample.Migrations
{
    [DbContext(typeof(AppUserRolesContext))]
    partial class AppUserRolesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.HasSequence<int>("seqAppRole");

            modelBuilder.HasSequence<int>("seqAppUser");

            modelBuilder.Entity("EDennis.AspNetUtils.AppRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValueSql("(NEXT VALUE FOR [seqAppRole])");

                    b.Property<string>("RoleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SysEnd")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysEnd");

                    b.Property<Guid>("SysGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("SysStart")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysStart");

                    b.Property<string>("SysUser")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AppRole", (string)null);

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("AppRole", "dbo_history");
                                ttb
                                    .HasPeriodStart("SysStart")
                                    .HasColumnName("SysStart");
                                ttb
                                    .HasPeriodEnd("SysEnd")
                                    .HasColumnName("SysEnd");
                            }));

                    b.HasData(
                        new
                        {
                            Id = -1,
                            RoleName = "IT",
                            SysGuid = new Guid("00000001-0000-0000-0000-000000000001")
                        },
                        new
                        {
                            Id = -2,
                            RoleName = "admin",
                            SysGuid = new Guid("00000002-0000-0000-0000-000000000002")
                        },
                        new
                        {
                            Id = -3,
                            RoleName = "user",
                            SysGuid = new Guid("00000003-0000-0000-0000-000000000003")
                        },
                        new
                        {
                            Id = -4,
                            RoleName = "readonly",
                            SysGuid = new Guid("00000004-0000-0000-0000-000000000004")
                        },
                        new
                        {
                            Id = -5,
                            RoleName = "disabled",
                            SysGuid = new Guid("00000005-0000-0000-0000-000000000005")
                        });
                });

            modelBuilder.Entity("EDennis.AspNetUtils.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValueSql("(NEXT VALUE FOR [seqAppUser])");

                    b.Property<int?>("RoleId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SysEnd")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysEnd");

                    b.Property<Guid>("SysGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("SysStart")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysStart");

                    b.Property<string>("SysUser")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AppUser", (string)null);

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("AppUser", "dbo_history");
                                ttb
                                    .HasPeriodStart("SysStart")
                                    .HasColumnName("SysStart");
                                ttb
                                    .HasPeriodEnd("SysEnd")
                                    .HasColumnName("SysEnd");
                            }));

                    b.HasData(
                        new
                        {
                            Id = -1,
                            RoleId = -1,
                            SysGuid = new Guid("00000001-0000-0000-0000-000000000001"),
                            UserName = "Starbuck"
                        },
                        new
                        {
                            Id = -2,
                            RoleId = -2,
                            SysGuid = new Guid("00000002-0000-0000-0000-000000000002"),
                            UserName = "Maria"
                        },
                        new
                        {
                            Id = -3,
                            RoleId = -3,
                            SysGuid = new Guid("00000003-0000-0000-0000-000000000003"),
                            UserName = "Darius"
                        },
                        new
                        {
                            Id = -4,
                            RoleId = -4,
                            SysGuid = new Guid("00000004-0000-0000-0000-000000000004"),
                            UserName = "Huan"
                        },
                        new
                        {
                            Id = -5,
                            RoleId = -5,
                            SysGuid = new Guid("00000005-0000-0000-0000-000000000005"),
                            UserName = "Jack"
                        });
                });

            modelBuilder.Entity("EDennis.AspNetUtils.AppUser", b =>
                {
                    b.HasOne("EDennis.AspNetUtils.AppRole", "AppRole")
                        .WithMany("AppUsers")
                        .HasForeignKey("RoleId");

                    b.Navigation("AppRole");
                });

            modelBuilder.Entity("EDennis.AspNetUtils.AppRole", b =>
                {
                    b.Navigation("AppUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
