﻿// <auto-generated />
using System;
using Landing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Landing.Infrastructure.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250404200041_AddBirthDateToUser")]
    partial class AddBirthDateToUser
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CuratedEventUser", b =>
                {
                    b.Property<int>("CuratedEventId")
                        .HasColumnType("integer");

                    b.Property<int>("CuratorsId")
                        .HasColumnType("integer");

                    b.HasKey("CuratedEventId", "CuratorsId");

                    b.HasIndex("CuratorsId");

                    b.ToTable("EventCurators", (string)null);
                });

            modelBuilder.Entity("Landing.Core.Models.Events.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Events");

                    b.HasDiscriminator().HasValue("Event");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Landing.Core.Models.Events.EventAttendance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ConfirmedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("EventId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("boolean");

                    b.Property<int>("PointsAwarded")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("UserId");

                    b.ToTable("EventAttendances");
                });

            modelBuilder.Entity("Landing.Core.Models.News", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<DateTime>("PublishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("News");
                });

            modelBuilder.Entity("Landing.Core.Models.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("Revoked")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("Landing.Core.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Landing.Core.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<bool>("IsEmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Points")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("integer");

                    b.Property<int?>("RegularEventId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RegularEventId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Landing.Core.Models.UserPointsTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("Points")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserPointsTransactions");
                });

            modelBuilder.Entity("Landing.Core.Models.UserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Landing.Core.Models.Events.CuratedEvent", b =>
                {
                    b.HasBaseType("Landing.Core.Models.Events.Event");

                    b.HasDiscriminator().HasValue("Curated");
                });

            modelBuilder.Entity("Landing.Core.Models.Events.OfflineEvent", b =>
                {
                    b.HasBaseType("Landing.Core.Models.Events.Event");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CustomHtmlTemplate")
                        .HasColumnType("text");

                    b.Property<decimal>("Latitude")
                        .HasPrecision(9, 6)
                        .HasColumnType("numeric(9,6)");

                    b.Property<decimal>("Longitude")
                        .HasPrecision(9, 6)
                        .HasColumnType("numeric(9,6)");

                    b.HasDiscriminator().HasValue("Offline");
                });

            modelBuilder.Entity("Landing.Core.Models.Events.RegularEvent", b =>
                {
                    b.HasBaseType("Landing.Core.Models.Events.Event");

                    b.HasDiscriminator().HasValue("Regular");
                });

            modelBuilder.Entity("CuratedEventUser", b =>
                {
                    b.HasOne("Landing.Core.Models.Events.CuratedEvent", null)
                        .WithMany()
                        .HasForeignKey("CuratedEventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Landing.Core.Models.User", null)
                        .WithMany()
                        .HasForeignKey("CuratorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Landing.Core.Models.Events.EventAttendance", b =>
                {
                    b.HasOne("Landing.Core.Models.Events.Event", "Event")
                        .WithMany("Attendances")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Landing.Core.Models.User", "User")
                        .WithMany("Attendances")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Landing.Core.Models.RefreshToken", b =>
                {
                    b.HasOne("Landing.Core.Models.User", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Landing.Core.Models.User", b =>
                {
                    b.HasOne("Landing.Core.Models.Events.RegularEvent", null)
                        .WithMany("Participants")
                        .HasForeignKey("RegularEventId");
                });

            modelBuilder.Entity("Landing.Core.Models.UserPointsTransaction", b =>
                {
                    b.HasOne("Landing.Core.Models.User", "User")
                        .WithMany("PointsTransactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Landing.Core.Models.UserRole", b =>
                {
                    b.HasOne("Landing.Core.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Landing.Core.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Landing.Core.Models.Events.Event", b =>
                {
                    b.Navigation("Attendances");
                });

            modelBuilder.Entity("Landing.Core.Models.Role", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("Landing.Core.Models.User", b =>
                {
                    b.Navigation("Attendances");

                    b.Navigation("PointsTransactions");

                    b.Navigation("RefreshTokens");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("Landing.Core.Models.Events.RegularEvent", b =>
                {
                    b.Navigation("Participants");
                });
#pragma warning restore 612, 618
        }
    }
}
