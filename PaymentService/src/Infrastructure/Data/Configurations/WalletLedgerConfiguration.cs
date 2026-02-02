using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Data.Configurations
{
    public class WalletLedgerConfiguration : BaseEntityConfiguration<WalletLedger>
    {
        public override void Configure(EntityTypeBuilder<WalletLedger> builder)
        {
            base.Configure(builder);

            builder.ToTable("WalletLedgers");

            builder.Property(wl => wl.WalletId)
                .IsRequired();

            builder.Property(wl => wl.Direction)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(wl => wl.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(wl => wl.ReferenceType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(wl => wl.ReferenceId)
                .IsRequired();

            builder.Property(wl => wl.BalanceAfter)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(wl => wl.Description)
                .HasConversion<string>()
                .HasMaxLength(500)
                .IsRequired(false);

            builder.HasIndex(x => x.WalletId);

        }
    }
}
