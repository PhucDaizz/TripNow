using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialService.Domain.Entities;

namespace SocialService.Infrastructure.Data.Configurations
{
    public class PostLikeConfiguration : BaseEntityConfiguration<PostLike>
    {
        public override void Configure(EntityTypeBuilder<PostLike> builder)
        {
            base.Configure(builder);

            builder.ToTable("PostLikes");

            builder.Property(x => x.PostId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();

            builder.HasIndex(x => new { x.PostId, x.UserId })
                   .IsUnique()
                   .HasDatabaseName("IX_PostLikes_PostId_UserId_Unique");

            builder.HasIndex(x => x.UserId)
                   .HasDatabaseName("IX_PostLikes_UserId");

            builder.HasOne<Post>()
                   .WithMany()
                   .HasForeignKey(x => x.PostId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
