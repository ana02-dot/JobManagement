using JobManagement.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace JobManagement.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    public static void ConfigureBaseEntity<T>(this ModelBuilder modelBuilder) where T : BaseEntity
    {
        modelBuilder.Entity<T>(entity =>
        {
            // Configure BaseEntity properties
            entity.Property(e => e.Id).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired(false);
            entity.Property(e => e.UpdatedBy).HasMaxLength(200);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            // Configure BaseEntity Creator relationship
            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });
    }
}