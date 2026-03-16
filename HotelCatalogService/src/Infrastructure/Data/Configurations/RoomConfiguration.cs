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

            builder.HasIndex(r => r.FloorId)
                   .HasDatabaseName("IX_Rooms_FloorId");

            builder.HasIndex(r => r.RoomTypeId)
                   .HasDatabaseName("IX_Rooms_RoomTypeId");

            builder.HasIndex(r => new { r.FloorId, r.RoomName })
                   .IsUnique()
                   .HasDatabaseName("IX_Rooms_FloorId_RoomName");

            builder.HasIndex(r => new { r.RoomTypeId, r.Status })
                   .HasDatabaseName("IX_Rooms_RoomTypeId_Status");
        }
    }
}
