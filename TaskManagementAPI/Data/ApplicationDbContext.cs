using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔹 Define Unique Constraint for User Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // 🔹 Define Relationship between TaskItem and User
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Assignee)
                .WithMany()
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Seed Users with Plain Text Passwords (⚠️ NOT SECURE FOR PRODUCTION)
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FullName = "Admin", Email = "admin@gmail.com", Password = "admin123", Role = "Admin" },
                new User { Id = 2, FullName = "Manager", Email = "manager@gmail.com", Password = "manager123", Role = "Manager" },
                new User { Id = 3, FullName = "User", Email = "user@gmail.com", Password = "user123", Role = "User" }
            );
        }
    }
}
