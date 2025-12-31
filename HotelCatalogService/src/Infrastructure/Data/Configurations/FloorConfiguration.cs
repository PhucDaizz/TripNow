using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class FloorConfiguration : BaseEntityConfiguration<Floor>
    {
        public override void Configure(EntityTypeBuilder<Floor> builder)
        {
            base.Configure(builder);
            builder.ToTable("Floors");

            builder.Property(f => f.FloorNumber).IsRequired();

            builder.HasMany(f => f.Rooms)
                   .WithOne()
                   .HasForeignKey(x => x.FloorId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(h => h.Rooms).HasField("_rooms");
        }
    }
}
