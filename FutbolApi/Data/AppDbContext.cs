using Microsoft.EntityFrameworkCore;
using FutbolApi.Models;

namespace FutbolApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public ApplicationDbContext()
        {
           
        }

        public DbSet<League> Leagues { get; set; }
        public DbSet<PlayerData> Players { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<League>()
                .Property(l => l.Id)
                .ValueGeneratedOnAdd(); // League Id is auto-incremented

            modelBuilder.Entity<PlayerData>()
                .Property(p => p.Id)
                .ValueGeneratedNever(); // Id is explicitly set

            modelBuilder.Entity<Log>()
                .Property(l => l.Id)
                .ValueGeneratedOnAdd(); // Log Id is auto-incremented
        }
    }
}
