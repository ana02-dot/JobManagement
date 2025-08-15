using JobManagement.Domain.Entities;
using JobManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace JobManagement.Infrastructure.Data;

public class JobManagementDbContext : DbContext
{
    public JobManagementDbContext(DbContextOptions<JobManagementDbContext> options) : base(options)
    {
    }
    
    public DbSet<Job> Jobs { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Applications> JobApplications { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PersonalNumber).IsRequired().HasMaxLength(11);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PhoneNumber).HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).HasConversion<int>();

                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.PersonalNumber).IsUnique();
                entity.HasIndex(e => e.PhoneNumber).IsUnique();

                entity.HasMany(e => e.ReviewedApplications)
                      .WithOne(e => e.ReviewedBy)
                      .HasForeignKey(e => e.ReviewedByUserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Job Configuration
            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Requirements).IsRequired();
                entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");

                entity.HasMany(e => e.Applications)
                      .WithOne(e => e.Job)
                      .HasForeignKey(e => e.JobId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // JobApplication Configuration
            modelBuilder.Entity<Applications>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.HasIndex(e => new { e.JobId, e.ApplicantId }).IsUnique();
            });
            
        }
}