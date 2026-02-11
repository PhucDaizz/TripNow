using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialService.Domain.Entities;

namespace SocialService.Infrastructure.Data.Configurations
{
    public class ReviewConfiguration : BaseEntityConfiguration<Review>
    {
        public override void Configure(EntityTypeBuilder<Review> builder)
        {
            base.Configure(builder);

            builder.ToTable("Reviews");


            builder.Property(x => x.Rating)
                   .HasColumnType("decimal(2,1)")
                   .IsRequired();

            builder.Property(x => x.TargetType)
                   .HasConversion<string>()
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.BookingId)
                   .IsRequired(false);

            builder.HasIndex(x => new { x.TargetId, x.TargetType })
                   .HasDatabaseName("IX_Reviews_Target_Performance");

            builder.HasIndex(x => x.PostId)
                   .IsUnique()
                   .HasDatabaseName("IX_Reviews_PostId_Unique");

            builder.HasIndex(x => x.BookingId)
                   .HasDatabaseName("IX_Reviews_BookingId");
        }
    }
}
