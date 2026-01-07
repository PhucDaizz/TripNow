using BookingService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Infrastructure.Data.Configurations
{
    public class BookingCancellationConfiguration : BaseEntityConfiguration<BookingCancellation>
    {
        public override void Configure(EntityTypeBuilder<BookingCancellation> builder)
        {
            base.Configure(builder);
            builder.ToTable("BookingCancellations");
            builder.HasKey(bc => bc.Id);

            builder.Property(bc => bc.RefundAmount).HasColumnType("decimal(18,2)");

            builder.Property(bc => bc.Reason).HasMaxLength(500);

            builder.Property(bc => bc.CancelledBy).HasConversion<int>();
            builder.Property(bc => bc.RefundPolicyType).HasConversion<int>();
        }
    }
}
