using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Data.Configurations
{
    public class RefundRequestConfiguration : BaseEntityConfiguration<RefundRequest>
    {
        public override void Configure(EntityTypeBuilder<RefundRequest> builder)
        {
            base.Configure(builder);

            builder.ToTable("RefundRequests");

            builder.Property(rr => rr.BookingId)
                .IsRequired();

            builder.Property(rr => rr.UserId)
                .IsRequired();

            builder.Property(rr => rr.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(rr => rr.OriginalPaymentTransactionId)
                .IsRequired();

            builder.Property(rr => rr.OriginalGatewayTransactionRef)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(rr => rr.RefundPaymentTransactionId);

            builder.Property(rr => rr.RefundGatewayTransactionRef)
                .HasMaxLength(100);

            builder.Property(rr => rr.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(rr => rr.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(rr => rr.AdminNote)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(rr => rr.ProcessedAt)
                .IsRequired(false);
            
            builder.HasIndex(rr => rr.BookingId);
            builder.HasIndex(rr => rr.UserId);
            builder.HasIndex(rr => rr.Status);
            builder.HasIndex(rr => rr.OriginalGatewayTransactionRef);

        }
    }
}
