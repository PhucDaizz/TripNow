using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Data.Configurations
{
    public class EscrowAccountConfiguration : BaseEntityConfiguration<EscrowAccount>
    {
        public override void Configure(EntityTypeBuilder<EscrowAccount> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.BookingId)
                .IsRequired();

            builder.Property(e => e.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(e => e.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(e => e.ProviderFee)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasIndex(e => e.BookingId);
        }
    }
}
