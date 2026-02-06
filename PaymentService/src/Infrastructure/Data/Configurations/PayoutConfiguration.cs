using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Data.Configurations
{
    public class PayoutConfiguration : BaseEntityConfiguration<Payout>
    {
        public override void Configure(EntityTypeBuilder<Payout> builder)
        {
            base.Configure(builder);

            builder.ToTable("Payouts");

            builder.Property(p => p.SettlementId)
                .IsRequired();

            builder.Property(p => p.BankInfo)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.TransactionReference)
                .HasMaxLength(250);

            builder.Property(p => p.FailureReason)
                .HasMaxLength(500);

            builder.Property(p => p.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne<SettlementPeriod>()
               .WithMany() 
               .HasForeignKey(p => p.SettlementId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.Id);
            builder.HasIndex(x => x.SettlementId);
        }
    }
}
