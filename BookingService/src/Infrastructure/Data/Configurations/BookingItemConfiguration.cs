using BookingService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Infrastructure.Data.Configurations
{
    public class BookingItemConfiguration : BaseEntityConfiguration<BookingItem>
    {
        public override void Configure(EntityTypeBuilder<BookingItem> builder)
        {
            base.Configure(builder);

            builder.ToTable("BookingItems");
            builder.HasKey(bi => bi.Id);

            builder.Property(bi => bi.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            // --- RELATIONSHIP: BookingItem -> RoomAssignments (1-N) ---
            builder.HasMany(bi => bi.Assignments)
                   .WithOne()
                   .HasForeignKey(ra => ra.BookingItemId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Map vào biến private _assignments
            builder.Navigation(bi => bi.Assignments)
                   .HasField("_assignments")
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
