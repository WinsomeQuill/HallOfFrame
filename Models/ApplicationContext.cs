using Microsoft.EntityFrameworkCore;

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
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=HOF_Test;Username=artem;Password=Kukushka1337");
        }
    }
}
