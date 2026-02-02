using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Data.Configurations
{
    public class SettlementItemConfiguration : BaseEntityConfiguration<SettlementItem>
    {
        public override void Configure(EntityTypeBuilder<SettlementItem> builder)
        {
            base.Configure(builder);

            builder.ToTable("SettlementItems");

            builder.Property(x => x.SettlementId)
                .IsRequired();

            builder.Property(x => x.BookingId)
                .IsRequired();

            builder.Property(x => x.GrossAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.CommissionAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.NetAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Type)
                .HasConversion<int>()
                .IsRequired();

            builder.HasIndex(x => x.SettlementId);

            builder.HasIndex(x => x.BookingId);

        }
    }
}
