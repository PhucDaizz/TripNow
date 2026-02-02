using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Data.Configurations
{
    public class SettlementPeriodConfiguration : BaseEntityConfiguration<SettlementPeriod>
    {
        public override void Configure(EntityTypeBuilder<SettlementPeriod> builder)
        {
            base.Configure(builder);

            builder.ToTable("SettlementPeriods");

            builder.Property(sp => sp.OwnerId)
                .IsRequired();

            builder.Property(sp => sp.TotalGross)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(sp => sp.TotalCommission)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(sp => sp.TotalNetPayable)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(sp => sp.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(sp => sp.PeriodFrom)
                .HasColumnType("datetime(6)")
                .IsRequired();

            builder.Property(sp => sp.PeriodTo)
                .HasColumnType("datetime(6)")
                .IsRequired();

            builder.HasMany(sp => sp.Payouts)
                .WithOne()
                .HasForeignKey(x => x.SettlementId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(sp => sp.SettlementItems)
                .WithOne()
                .HasForeignKey(x => x.SettlementId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(sp => sp.Payouts)
                .HasField("_payouts")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(sp => sp.SettlementItems)
                .HasField("_settlementItems")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(x => x.OwnerId);
        }
    }
}
