using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class PromotionUsageConfiguration : BaseEntityConfiguration<PromotionUsage>
    {
        public override void Configure(EntityTypeBuilder<PromotionUsage> builder)
        {
            base.Configure(builder);

            builder.ToTable("PromotionUsages");

            builder.Property(x => x.PromotionId).IsRequired();
            builder.Property(x => x.BookingId).IsRequired();
            builder.Property(x => x.UserId).IsRequired();

            builder.HasIndex(x => new { x.PromotionId, x.BookingId })
                   .IsUnique();
        }
    }
}
