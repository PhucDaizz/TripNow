using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialService.Domain.Entities;

namespace SocialService.Infrastructure.Data.Configurations
{
    public class PostImageConfiguration : BaseEntityConfiguration<PostImage>
    {
        public override void Configure(EntityTypeBuilder<PostImage> builder)
        {
            base.Configure(builder);

            builder.ToTable("PostImages");

            builder.Property(x => x.ImageUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.PublicId)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.Caption)
                   .HasMaxLength(255) 
                   .IsRequired(false);

            builder.Property(x => x.SortOrder)
                   .HasDefaultValue(0);


            builder.HasIndex(x => x.PostId)
                   .HasDatabaseName("IX_PostImages_PostId");
        }
    }
}
