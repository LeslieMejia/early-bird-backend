using EarlyBirdAPI.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace EarlyBirdAPI.Model
{
    public class EarlyBirdDbContext : DbContext
    {
        public EarlyBirdDbContext(DbContextOptions<EarlyBirdDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }  // Maps to 'users' table
        public DbSet<Job> Job { get; set; }
        public DbSet<Resume> Resume { get; set; }
        public DbSet<JobApplication> JobApplication { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PostgreSQL enums
            modelBuilder
                .HasPostgresEnum("application_status", new[] { "applied", "reviewed", "interview", "rejected" })
                .HasPostgresEnum("jobposting_status", new[] { "active", "expired","closed" })
                .HasPostgresEnum("user_role", new[] { "employer", "jobseeker" });

            // Mapping UserRole as string in DB
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<JobApplication>()
                .Property(j => j.Status)
                .HasConversion<string>();

            // Optional: Ensure email is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

    }
}
