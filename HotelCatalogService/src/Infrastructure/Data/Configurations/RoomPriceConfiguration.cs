using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class RoomPriceConfiguration : BaseEntityConfiguration<RoomPrice>
    {
        public override void Configure(EntityTypeBuilder<RoomPrice> builder)
        {
            base.Configure(builder);
            builder.ToTable("RoomPrices");

            builder.Property(rp => rp.Date).HasColumnType("date").IsRequired();
            builder.Property(rp => rp.Price).HasColumnType("decimal(18,2)").IsRequired();
        }
    }
}
