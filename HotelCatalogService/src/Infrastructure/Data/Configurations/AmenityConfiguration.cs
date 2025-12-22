using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class AmenityConfiguration : BaseEntityConfiguration<Amenity>
    {
        public override void Configure(EntityTypeBuilder<Amenity> builder)
        {
            base.Configure(builder);

            builder.ToTable("Amenities");

            builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
            builder.Property(a => a.Icon).HasColumnType("varchar(50)");
        }
    }
}
