using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Data.Configurations
{
    public class PaymentTransactionConfiguration : BaseEntityConfiguration<PaymentTransaction>
    {
        public override void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            base.Configure(builder);

            builder.Property(pt => pt.PayerUserId)
                .IsRequired(false);

            builder.Property(pt => pt.BookingId)
                .IsRequired();

            builder.Property(pt => pt.Amount)
                .HasPrecision(18,2)
                .IsRequired();

            builder.Property(pt => pt.Provider)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(pt => pt.MerchantRef)
                .IsRequired(false)
                .HasMaxLength(100);

            builder.Property(pt => pt.ProviderTxnId)
                .IsRequired(false)
                .HasMaxLength(100);

            builder.Property(pt => pt.ProviderFee)
                .HasPrecision(18,2)
                .IsRequired(false);

            builder.Property(pt => pt.RawResponse)
                .IsRequired(false);

            builder.Property(pt => pt.TransactionStatus)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(pt => pt.FailureReason)
                .IsRequired(false);

            builder.Property(pt => pt.PaymentDate)
                .HasColumnType("datetime(6)")
                .IsRequired(false);

            builder.HasIndex(pt => pt.BookingId);

            builder.HasIndex(pt => pt.PayerUserId);

        }
    }
}
