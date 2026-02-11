using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialService.Domain.Entities;

namespace SocialService.Infrastructure.Data.Configurations
{
    public class UserFollowConfiguration : BaseEntityConfiguration<UserFollow>
    {
        public override void Configure(EntityTypeBuilder<UserFollow> builder)
        {
            base.Configure(builder);

            builder.ToTable("UserFollows");

            builder.Property(x => x.FollowerId)
                   .IsRequired();

            builder.Property(x => x.TargetId)
                   .IsRequired();

            builder.Property(x => x.Type)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.HasIndex(x => new { x.FollowerId, x.TargetId, x.Type })
                   .IsUnique()
                   .HasDatabaseName("IX_UserFollows_Unique_Constraint");

            builder.HasIndex(x => x.FollowerId)
                   .HasDatabaseName("IX_UserFollows_FollowerId");

            builder.HasIndex(x => new { x.TargetId, x.Type })
                   .HasDatabaseName("IX_UserFollows_Target_Type");
        }
    }
}
