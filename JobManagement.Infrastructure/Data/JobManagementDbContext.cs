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
    public DbSet<JobApplication> JobApplications { get; set; }
    
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

                entity.HasMany(e => e.Applications)
                      .WithOne(e => e.Applicant)
                      .HasForeignKey(e => e.ApplicantId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.CreatedJobs)
                      .WithOne(e => e.CreatedBy)
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Restrict);

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
                entity.Property(e => e.SalaryMin).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SalaryMax).HasColumnType("decimal(18,2)");

                entity.HasMany(e => e.Applications)
                      .WithOne(e => e.Job)
                      .HasForeignKey(e => e.JobId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // JobApplication Configuration
            modelBuilder.Entity<JobApplication>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CoverLetter).IsRequired();
                entity.Property(e => e.Status).HasConversion<int>();

                entity.HasIndex(e => new { e.JobId, e.ApplicantId }).IsUnique();
            });

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Admin User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    PersonalNumber = "00000000000", // Placeholder for admin
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "admin@jobmanagement.ge",
                    PhoneNumber = "+995555000000",
                    PasswordHash = "hashed_admin_password", // Should be properly hashed
                    Role = UserRole.Admin,
                    IsPersonalNumberVerified = true,
                    PersonalNumberVerifiedAt = DateTime.UtcNow,
                    IsEmailVerified = true,
                    EmailVerifiedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            );
        }
}