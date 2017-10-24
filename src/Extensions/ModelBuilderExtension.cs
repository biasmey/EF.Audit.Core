using Microsoft.EntityFrameworkCore;

namespace EF.Audit.Core.Extensions
{
    public static class ModelBuilderExtension
    {
        
        public static ModelBuilder EnableAudit(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>(b =>
            {
                b.Property(c => c.Created);
                b.Property(c => c.EntityId);
                b.Property(c => c.EntityJson);
                b.Property(c => c.Operation).IsRequired();
                b.Property(c => c.EntityFullName).HasMaxLength(128);
                b.Property(c => c.Table).IsRequired().HasMaxLength(128);
                b.Property(c => c.TransssactionId);
                b.Property(c => c.User);
            });
            modelBuilder.Entity<AuditLog>().ToTable("AuditLogs");

            modelBuilder.Entity<PropertyAudit>(b =>
            {
                b.Property(c => c.NewValue).IsRequired();
                b.Property(c => c.OldValue).IsRequired();
                b.Property(c => c.PropertyName);
            });
            modelBuilder.Entity<PropertyAudit>().ToTable("PropertyAudits");

            modelBuilder.Entity<AuditLog>().HasMany(q => q.PropertiesAudits).WithOne(q => q.AuditLog).IsRequired();
            return modelBuilder;
        }
    }
}
