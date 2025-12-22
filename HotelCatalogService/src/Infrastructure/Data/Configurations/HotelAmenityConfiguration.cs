using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class HotelAmenityConfiguration : BaseEntityConfiguration<HotelAmenity>
    {
        public override void Configure(EntityTypeBuilder<HotelAmenity> builder)
        {
            base.Configure(builder);
            builder.ToTable("HotelAmenities");

            builder.Property(ha => ha.Description).HasMaxLength(100);
            builder.Property(ha => ha.IsFree).IsRequired();

            builder.HasOne<Amenity>() 
                   .WithMany()
                   .HasForeignKey(ha => ha.AmenityId) 
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(ha => new { ha.HotelId, ha.AmenityId })
                   .IsUnique();

        }
    }
}
