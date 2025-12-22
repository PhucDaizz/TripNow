using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class HotelImageConfiguration : BaseEntityConfiguration<HotelImage>
    {
        public override void Configure(EntityTypeBuilder<HotelImage> builder)
        {
            base.Configure(builder);
            builder.ToTable("HotelImages");

            builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired(); 
            builder.Property(x => x.Caption).HasMaxLength(100);
            builder.Property(x => x.IsThumbnail).IsRequired();
        }
    }
}
