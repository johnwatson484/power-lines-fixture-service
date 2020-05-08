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
    [Migration("20200508143318_AddRecommendations")]
    partial class AddRecommendations
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
                        .HasColumnName("fixtureId")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("AwayOddsAverage")
                        .HasColumnName("awayOddsAverage")
                        .HasColumnType("numeric");

                    b.Property<string>("AwayTeam")
                        .HasColumnName("awayTeam")
                        .HasColumnType("text");

                    b.Property<DateTime>("Date")
                        .HasColumnName("date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Division")
                        .HasColumnName("division")
                        .HasColumnType("text");

                    b.Property<decimal>("DrawOddsAverage")
                        .HasColumnName("drawOddsAverage")
                        .HasColumnType("numeric");

                    b.Property<decimal>("HomeOddsAverage")
                        .HasColumnName("homeOddsAverage")
                        .HasColumnType("numeric");

                    b.Property<string>("HomeTeam")
                        .HasColumnName("homeTeam")
                        .HasColumnType("text");

                    b.HasKey("FixtureId");

                    b.HasIndex("Date", "HomeTeam", "AwayTeam")
                        .IsUnique();

                    b.ToTable("fixtures");
                });

            modelBuilder.Entity("PowerLinesFixtureService.Models.MatchOdds", b =>
                {
                    b.Property<int>("MatchOddsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("matchOddsId")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<decimal>("Away")
                        .HasColumnName("away")
                        .HasColumnType("numeric");

                    b.Property<int>("AwayGoals")
                        .HasColumnName("expectedAwayGoals")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Calculated")
                        .HasColumnName("calculated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal>("Draw")
                        .HasColumnName("draw")
                        .HasColumnType("numeric");

                    b.Property<decimal>("ExpectedGoals")
                        .HasColumnName("expectedGoals")
                        .HasColumnType("numeric");

                    b.Property<int>("FixtureId")
                        .HasColumnName("fixtureId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Home")
                        .HasColumnName("home")
                        .HasColumnType("numeric");

                    b.Property<int>("HomeGoals")
                        .HasColumnName("expectedHomeGoals")
                        .HasColumnType("integer");

                    b.Property<string>("LowerRecommended")
                        .HasColumnName("lowerRecommended")
                        .HasColumnType("text");

                    b.Property<string>("Recommended")
                        .HasColumnName("recommended")
                        .HasColumnType("text");

                    b.HasKey("MatchOddsId");

                    b.HasIndex("FixtureId")
                        .IsUnique();

                    b.ToTable("match_odds");
                });

            modelBuilder.Entity("PowerLinesFixtureService.Models.MatchOdds", b =>
                {
                    b.HasOne("PowerLinesFixtureService.Models.Fixture", null)
                        .WithOne("MatchOdds")
                        .HasForeignKey("PowerLinesFixtureService.Models.MatchOdds", "FixtureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
