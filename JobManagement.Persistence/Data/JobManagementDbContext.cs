using JobManagement.Domain.Entities;
using JobManagement.Domain.Enums;
using JobManagement.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace JobManagement.Persistence.Data;

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

        modelBuilder.ConfigureBaseEntity<User>();
        modelBuilder.ConfigureBaseEntity<Job>();
        modelBuilder.ConfigureBaseEntity<Applications>();

        // User Configuration - Only entity-specific configurations
        modelBuilder.Entity<User>(entity =>
        {
            // Entity-specific properties only
            entity.Property(e => e.PersonalNumber).IsRequired().HasMaxLength(11);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).HasConversion<int>();

            // Indexes
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.PersonalNumber).IsUnique();
            entity.HasIndex(e => e.PhoneNumber).IsUnique();

            // Entity-specific relationships only
            // User -> Jobs (as creator) - Override the base Creator relationship
            entity.HasOne(e => e.Creator)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired();

            entity.HasMany(e => e.CreatedJobs)
                  .WithOne(j => j.Creator)
                  .HasForeignKey(j => j.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            // User -> Applications (as applicant)
            entity.HasMany(e => e.Applications)
                  .WithOne(a => a.Applicant)
                  .HasForeignKey(a => a.ApplicantId)
                  .OnDelete(DeleteBehavior.Cascade);

            // User -> Applications (as reviewer)
            entity.HasMany(e => e.ReviewedApplications)
                  .WithOne(a => a.ReviewedBy)
                  .HasForeignKey(a => a.ReviewedByUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Job Configuration - Only entity-specific configurations
        modelBuilder.Entity<Job>(entity =>
        {
            // Entity-specific properties only
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Requirements).IsRequired();
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ApplicationDeadline).IsRequired();

            // Override BaseEntity Creator relationship for specific behavior
            entity.HasOne(e => e.Creator)
                  .WithMany(u => u.CreatedJobs)
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired();

            // Job -> Applications relationship
            entity.HasMany(e => e.Applications)
                  .WithOne(a => a.Job)
                  .HasForeignKey(a => a.JobId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Applications Configuration - Only entity-specific configurations
        modelBuilder.Entity<Applications>(entity =>
        {
            // Fix: Applications should have its own Id, not use JobId as primary key
            entity.HasKey(e => e.Id); // This was wrong in your original: entity.HasKey(e => e.JobId);
            
            // Entity-specific properties
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.ReviewedByUserId).IsRequired(false); // Should be optional
            entity.Property(e => e.ApplicantId).IsRequired();

            // Applications -> Job relationship
            entity.HasOne(e => e.Job)
                  .WithMany(j => j.Applications)
                  .HasForeignKey(e => e.JobId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired();

            // Applications -> User (applicant) relationship
            entity.HasOne(e => e.Applicant)
                  .WithMany(u => u.Applications)
                  .HasForeignKey(e => e.ApplicantId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .IsRequired();

            // Applications -> User (reviewer) relationship
            entity.HasOne(e => e.ReviewedBy)
                  .WithMany(u => u.ReviewedApplications)
                  .HasForeignKey(e => e.ReviewedByUserId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .IsRequired(false);

            // Unique constraint: one application per user per job
            entity.HasIndex(e => new { e.JobId, e.ApplicantId }).IsUnique();
        });

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                PersonalNumber = "00000000000",
                FirstName = "System",
                LastName = "Admin",
                Email = "admin@jobmanagement.com",
                PhoneNumber = "000-000-0000",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                IsDeleted = false
            }
        );
    }
}