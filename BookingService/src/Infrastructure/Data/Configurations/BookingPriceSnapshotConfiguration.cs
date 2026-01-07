using BookingService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Infrastructure.Data.Configurations
{
    public class BookingPriceSnapshotConfiguration : BaseEntityConfiguration<BookingPriceSnapshot>
    {
        public override void Configure(EntityTypeBuilder<BookingPriceSnapshot> builder)
        {
            base.Configure(builder);
            builder.ToTable("BookingPriceSnapshots");
            builder.HasKey(bps => bps.Id);

            builder.Property(bps => bps.GrossAmount).HasColumnType("decimal(18,2)");
            builder.Property(bps => bps.PromotionAmount).HasColumnType("decimal(18,2)");
            builder.Property(bps => bps.VATAmount).HasColumnType("decimal(18,2)");
            builder.Property(bps => bps.ServiceFeeAmount).HasColumnType("decimal(18,2)");
            builder.Property(bps => bps.NetPayableByCustomer).HasColumnType("decimal(18,2)");
        }
    }
}
