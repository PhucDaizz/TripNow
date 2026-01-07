using BookingService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Infrastructure.Data.Configurations
{
    public class InventoryConfiguration : BaseEntityConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            base.Configure(builder);
            builder.ToTable("Inventories");
            builder.HasKey(x => x.Id);

            builder.Property(i => i.Date)
                   .HasColumnType("date") 
                   .IsRequired();

            builder.Property(i => i.TotalStock).IsRequired();
            builder.Property(i => i.SoldStock).IsRequired();

            // 2. UNIQUE INDEX (Rất quan trọng)
            // Đảm bảo mỗi loại phòng trong 1 ngày chỉ có đúng 1 record quản lý tồn kho
            builder.HasIndex(i => new { i.RoomTypeId, i.Date })
                   .IsUnique();

            // 3. CONCURRENCY TOKEN (Chống Overbooking)
            // Với MySQL, RowVersion thường dùng check bằng cột Timestamp hoặc ConcurrencyCheck
            builder.Property(i => i.RowVersion)
                   .IsRowVersion();

        }
    }
}
