using Microsoft.EntityFrameworkCore;
using PowerLinesFixtureService.Models;

namespace PowerLinesFixtureService.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Fixture> Fixtures { get; set; }
    public DbSet<MatchOdds> MatchOdds { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fixture>()
            .HasIndex(x => new { x.Date, x.HomeTeam, x.AwayTeam }).IsUnique();
    }
}
