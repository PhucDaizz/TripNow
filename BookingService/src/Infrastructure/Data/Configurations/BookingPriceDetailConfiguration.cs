using BookingService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Infrastructure.Data.Configurations
{
    public class BookingPriceDetailConfiguration : BaseEntityConfiguration<BookingPriceDetail>
    {
        public override void Configure(EntityTypeBuilder<BookingPriceDetail> builder)
        {
            base.Configure(builder);
            builder.ToTable("BookingPriceDetails");
            builder.HasKey(bpd => bpd.Id);

            builder.Property(bpd => bpd.Amount).HasColumnType("decimal(18,2)");

            builder.Property(bpd => bpd.Type)
                   .HasConversion<int>(); 

            builder.Property(bpd => bpd.Description)
                   .HasMaxLength(250);
        }
    }
}
