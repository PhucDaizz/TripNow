using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class RoomTypeImageConfiguration : BaseEntityConfiguration<RoomTypeImage>
    {
        public override void Configure(EntityTypeBuilder<RoomTypeImage> builder)
        {
            base.Configure(builder);
            builder.ToTable("RoomTypeImages");

            builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
        }
    }
}
