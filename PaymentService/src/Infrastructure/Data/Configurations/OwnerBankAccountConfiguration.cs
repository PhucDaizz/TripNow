using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;

namespace PaymentService.Infrastructure.Data.Configurations
{
    public class OwnerBankAccountConfiguration : BaseEntityConfiguration<OwnerBankAccount>
    {
        public override void Configure(EntityTypeBuilder<OwnerBankAccount> builder)
        {
            base.Configure(builder);
            builder.ToTable("OwnerBankAccounts");

            builder.Property(e => e.OwnerId)
                .IsRequired();

            builder.Property(e => e.BankName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.BankAccountNumber)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.BankAccountHolder)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.IsDefault)
                .IsRequired();

            builder.HasIndex(e => e.OwnerId);

            builder.HasIndex(e => new { e.OwnerId, e.IsDefault });
        }
    }
}
