﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PowerLinesFixtureService.Data;

namespace PowerLinesFixtureService.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20200105170212_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("PowerLinesFixtureService.Models.Fixture", b =>
                {
                    b.Property<int>("FixtureId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("AwayOddsAverage")
                        .HasColumnType("numeric");

                    b.Property<string>("AwayTeam")
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Division")
                        .HasColumnType("text");

                    b.Property<decimal>("DrawOddsAverage")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HomeOddsAverage")
                        .HasColumnType("numeric");

                    b.Property<string>("HomeTeam")
                        .HasColumnType("text");

                    b.HasKey("FixtureId");

                    b.ToTable("Fixtures");
                });
#pragma warning restore 612, 618
        }
    }
}