using BookingService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Infrastructure.Data.Configurations
{
    public class BookingConfiguration : BaseEntityConfiguration<Booking>
    {
        public override void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.HasKey(b => b.Id);

            // --- PROPERTIES ---

            // Decimal cho tiền tệ trong MySQL
            builder.Property(b => b.TotalAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(b => b.DiscountAmount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            // String Length (MySQL varchar)
            builder.Property(b => b.PromotionCode)
                   .HasMaxLength(50);

            // Enum Conversion (Lưu xuống DB dạng int/tinyint )
            builder.Property(b => b.Status)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(b => b.PaymentStatus)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(b => b.Source)
                   .HasConversion<int>()
                   .IsRequired();

            // DateOnly (MySQL DATE type) 
            builder.Property(b => b.CheckInDate).HasColumnType("date");
            builder.Property(b => b.CheckOutDate).HasColumnType("date");

            // --- RELATIONSHIPS & BACKING FIELDS (DDD) ---

            // 1. Booking -> BookingItems (1-N)
            builder.HasMany(b => b.Items)
                   .WithOne()
                   .HasForeignKey(bi => bi.BookingId)
                   .OnDelete(DeleteBehavior.Cascade); 

            // Map vào biến private _items
            builder.Navigation(b => b.Items)
                   .HasField("_items")
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            // 2. Booking -> BookingPriceDetails (1-N)
            builder.HasMany(b => b.PriceDetails)
                   .WithOne()
                   .HasForeignKey(pd => pd.BookingId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Map vào biến private _priceDetails
            builder.Navigation(b => b.PriceDetails)
                   .HasField("_priceDetails")
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            // 3. Booking -> BookingPriceSnapshot (1-1)
            builder.HasOne(b => b.PriceSnapshot)
                   .WithOne()
                   .HasForeignKey<BookingPriceSnapshot>(s => s.BookingId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 4. Booking -> BookingCancellation (1-0..1)
            builder.HasOne(b => b.Cancellation)
                   .WithOne()
                   .HasForeignKey<BookingCancellation>(c => c.BookingId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
