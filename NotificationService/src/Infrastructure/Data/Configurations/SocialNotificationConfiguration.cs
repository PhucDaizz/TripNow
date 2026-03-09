using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Data.Configurations
{
    public class SocialNotificationConfiguration : BaseEntityConfiguration<SocialNotification>
    {
        public override void Configure(EntityTypeBuilder<SocialNotification> builder)
        {
            base.Configure(builder);

            builder.Property(n => n.UserId)
                .IsRequired(true);

            builder.Property(n => n.ActionType)
                .IsRequired(true);

            builder.Property(n => n.ReferenceId)
                .HasMaxLength(100) 
                .IsRequired(true);

            builder.Property(n => n.LastActorId)
                .IsRequired(true);

            builder.Property(n => n.LastActorName)
                .HasMaxLength(100)
                .IsRequired(true);

            builder.Property(n => n.ActorCount)
                .HasDefaultValue(1)
                .IsRequired(true);

            builder.Property(n => n.IsRead)
                .HasDefaultValue(false)
                .IsRequired(true);

            builder.Property(n => n.ReadAt)
                .IsRequired(false);

           
            builder.HasIndex(n => new { n.UserId, n.CreatedAt })
                .HasDatabaseName("IX_SocialNotif_UserId_CreatedAt");

            builder.HasIndex(n => new { n.UserId, n.IsRead })
                .HasDatabaseName("IX_SocialNotif_UserId_IsRead");

            builder.HasIndex(n => new { n.UserId, n.ActionType, n.ReferenceId, n.IsRead })
                .HasDatabaseName("IX_SocialNotif_Aggregation");
        }
    }
}
