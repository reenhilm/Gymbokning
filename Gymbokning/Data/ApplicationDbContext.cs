using Gymbokning.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gymbokning.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<GymClass> GymClass => Set<GymClass>();
        public DbSet<ApplicationUserGymClass> ApplicationUserGymClass => Set<ApplicationUserGymClass>();
        public DbSet<ApplicationUser> Member => Set<ApplicationUser>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(v => v.GymClasses)
                .WithMany(s => s.ApplicationUsers)
                .UsingEntity<ApplicationUserGymClass>(
                va => va.HasOne(va => va.GymClass).WithMany(v => v.ApplicationUserGymClasses),
                va => va.HasOne(va => va.ApplicationUser).WithMany(v => v.ApplicationUserGymClasses));

            modelBuilder.Entity<GymClass>().HasData(
                new GymClass { Id = 1, Description = "GymClassDescription", Name = "GymClassName", Duration = new TimeSpan(1,0,0), StartTime = DateTime.Now.AddHours(1) }
            );
        }
    }
}