using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Data.Configurations
{
    public class NotificationConfiguration : BaseEntityConfiguration<Notification>
    {
        public override void Configure(EntityTypeBuilder<Notification> builder)
        {
            base.Configure(builder);

            builder.Property(n => n.UserId)
                .IsRequired(true);

            builder.Property(n => n.Title)
                .HasMaxLength(200)
                .IsRequired(true);

            builder.Property(n => n.Message)
                .HasMaxLength(500)
                .IsRequired(true);

            builder.Property(n => n.Type)
                .IsRequired(true);

            builder.Property(n => n.ReferenceId)
                .IsRequired(false)
                .HasMaxLength(100);

            builder.Property(n => n.IsRead)
                .HasDefaultValue(false);

            builder.Property(n => n.ReadAt)
                .IsRequired(false);

            builder.HasIndex(n => new { n.UserId, n.CreatedAt })
                .HasDatabaseName("IX_Notifications_UserId_CreatedAt");

            builder.HasIndex(n => new { n.UserId, n.IsRead })
                .HasDatabaseName("IX_Notifications_UserId_IsRead");

            builder.HasIndex(n => new { n.UserId, n.Type, n.ReferenceId })
                .HasDatabaseName("IX_Notifications_UserId_Type_RefId");

        }
    }
}
