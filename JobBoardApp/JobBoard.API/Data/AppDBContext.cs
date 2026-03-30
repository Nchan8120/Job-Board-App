using JobBoard.API.Models;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Interest> Interests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One user can post many jobs
            modelBuilder.Entity<Job>()
                .HasOne(j => j.PostedBy)
                .WithMany(u => u.Jobs)
                .HasForeignKey(j => j.PostedById)
                .OnDelete(DeleteBehavior.Cascade);

            // One user can express interest in many jobs
            modelBuilder.Entity<Interest>()
                .HasOne(i => i.User)
                .WithMany(u => u.Interests)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One job can have many interested users
            modelBuilder.Entity<Interest>()
                .HasOne(i => i.Job)
                .WithMany(j => j.Interests)
                .HasForeignKey(i => i.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // A user can only express interest in a job once
            modelBuilder.Entity<Interest>()
                .HasIndex(i => new { i.UserId, i.JobId })
                .IsUnique();
        }
    }
}