using Microsoft.EntityFrameworkCore;
using PowerLinesFixtureService.Models;

namespace PowerLinesFixtureService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Fixture> Fixtures { get; set; } 
        public DbSet<MatchOdds> MatchOdds { get; set; } 
    }
}
