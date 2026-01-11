using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Infrastructure.Data.Configurations
{
    public class InventoryCFConfiguration : BaseEntityConfiguration<Domain.Entities.InventoryConfiguration>
    {
        public override void Configure(EntityTypeBuilder<Domain.Entities.InventoryConfiguration> builder)
        {
            base.Configure(builder);

            builder.ToTable("InventoryConfiguration");
            builder.HasKey(bi => bi.Id);

            builder.Property(x => x.HotelId)
                   .IsRequired();

            builder.Property(x => x.RoomTypeId)
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(x => x.LastGeneratedDate)
                   .HasColumnType("date");

            builder.HasIndex(x => new { x.HotelId, x.RoomTypeId })
                   .IsUnique();

            builder.HasIndex(x => x.IsActive);
        }

    }
}
