using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Data.Configurations
{
    public class OwnerWalletConfiguration : BaseEntityConfiguration<OwnerWallet>
    {
        public override void Configure(EntityTypeBuilder<OwnerWallet> builder)
        {
            base.Configure(builder);

            builder.ToTable("OwnerWallets");

            builder.Property(ow => ow.OwnerId)
                   .IsRequired();

            builder.Property(ow => ow.AvailableBalance)
                    .HasPrecision(18,2)
                    .IsRequired();
            builder.Property(ow => ow.PendingBalance)
                    .HasPrecision(18,2)
                    .IsRequired();

            builder.Property(ow => ow.RowVersion)
                   .IsRowVersion();

            builder.HasMany(ow => ow.WalletLedgers)
                    .WithOne()
                    .HasForeignKey(wl => wl.WalletId)
                    .IsRequired();

            builder.Navigation(ow => ow.WalletLedgers)
                   .HasField("_walletLedgers")
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(ow => ow.OwnerId);
        }
    }
}
