using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialService.Domain.Entities;

namespace SocialService.Infrastructure.Data.Configurations
{
    public class PostConfiguration : BaseEntityConfiguration<Post>
    {
        public override void Configure(EntityTypeBuilder<Post> builder)
        {
            base.Configure(builder);

            builder.ToTable("Posts");

            builder.Property(x => x.Content)
                   .IsRequired()
                   .HasColumnType("longtext");

            builder.Property(c => c.UserId)
                .IsRequired(true);

            builder.Property(x => x.ThumbnailUrl)
                   .HasMaxLength(500)
                   .IsRequired(false);

            builder.Property(x => x.Type)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.Status)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.LikeCount).HasDefaultValue(0);
            builder.Property(x => x.CommentCount).HasDefaultValue(0);

            builder.Property(x => x.IsDeleted).HasDefaultValue(false);

            builder.Property(x => x.HotelId)
                   .IsRequired(false);

            var navigation = builder.Metadata.FindNavigation(nameof(Post.Images));
            if (navigation != null)
            {
                navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            }

            builder.HasMany(p => p.Images)
                   .WithOne() 
                   .HasForeignKey(img => img.PostId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(p => p.ReviewDetail)
                   .WithOne() 
                   .HasForeignKey<Review>(r => r.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.HotelId, x.Status, x.CreatedAt })
                   .HasDatabaseName("IX_Posts_Hotel_Feed");

            builder.HasIndex(x => new { x.UserId, x.Status })
                   .HasDatabaseName("IX_Posts_User_Profile");

            builder.HasIndex(x => new { x.Status, x.CreatedAt })
                   .HasDatabaseName("IX_Posts_Global_Feed");

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
