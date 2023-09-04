using Microsoft.EntityFrameworkCore;
using Serilog;

namespace HallOfFame_Test.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        public DbSet<Skill> Skills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasMany(p => p.skills)
                .WithMany()
                .UsingEntity(j => j.ToTable("PersonSkills"));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
            string? port = Environment.GetEnvironmentVariable("POSTGRES_PORT");
            string? dbName = Environment.GetEnvironmentVariable("POSTGRES_DB");
            string? user = Environment.GetEnvironmentVariable("POSTGRES_USER");
            string? password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            
            Log.Logger.Debug($"Host={host};Port={port};Database={dbName};Username={user};Password={password}");
            
            optionsBuilder.UseNpgsql($"Host={host};Port={port};Database={dbName};Username={user};Password={password}");
        }
    }
}
