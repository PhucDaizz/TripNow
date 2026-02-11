using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialService.Domain.Entities;

namespace SocialService.Infrastructure.Data.Configurations
{
    public class SavedPostConfiguration : BaseEntityConfiguration<SavedPost>
    {
        public override void Configure(EntityTypeBuilder<SavedPost> builder)
        {
            base.Configure(builder);

            builder.ToTable("SavedPosts");

            builder.Property(x => x.PostId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();

            builder.HasIndex(x => new { x.UserId, x.PostId })
                   .IsUnique()
                   .HasDatabaseName("IX_SavedPosts_UserId_PostId_Unique");

            builder.HasIndex(x => x.UserId)
                   .HasDatabaseName("IX_SavedPosts_UserId");

            builder.HasOne<Post>()
                   .WithMany()
                   .HasForeignKey(x => x.PostId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
