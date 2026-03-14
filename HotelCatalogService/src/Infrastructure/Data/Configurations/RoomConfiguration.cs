using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class RoomConfiguration : BaseEntityConfiguration<Room>
    {
        public override void Configure(EntityTypeBuilder<Room> builder)
        {
            base.Configure(builder);
            builder.ToTable("Rooms");

            builder.Property(r => r.RoomName).HasMaxLength(20).IsRequired();

            builder.Property(r => r.Status)
                   .HasConversion<byte>()
                   .IsRequired();

            builder.Property(x => x.AssignedToStaffId)
                    .IsRequired(false);

            builder.HasOne<RoomType>()
                   .WithMany()
                   .HasForeignKey(r => r.RoomTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
