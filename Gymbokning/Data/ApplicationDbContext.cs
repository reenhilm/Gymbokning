using Gymbokning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gymbokning.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole,
string>

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<GymClass> GymClass => Set<GymClass>();
        public DbSet<ApplicationUserGymClass> ApplicationUserGymClass => Set<ApplicationUserGymClass>();

        //TODO There is already inherited DbSet for ApplicationUser called Users
        //public DbSet<ApplicationUser> ApplicationUser => Set<ApplicationUser>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(v => v.GymClasses)
                .WithMany(s => s.ApplicationUsers)
                .UsingEntity<ApplicationUserGymClass>(
                va => va.HasOne(va => va.GymClass).WithMany(v => v.ApplicationUserGymClasses),
                va => va.HasOne(va => va.ApplicationUser).WithMany(v => v.ApplicationUserGymClasses));

            modelBuilder.Entity<ApplicationUser>()
                .Property<DateTime>("TimeOfRegistration")
                .HasDefaultValueSql("GetDate()"); ;

            //modelBuilder.Entity<IdentityRole>().HasData(                
            //    new IdentityRole {  }
            //);
        }
    }
}