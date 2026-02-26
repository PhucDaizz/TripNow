using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class CancellationRuleConfiguration : BaseEntityConfiguration<CancellationRule>
    {
        public override void Configure(EntityTypeBuilder<CancellationRule> builder)
        {
            base.Configure(builder);

            builder.ToTable("CancellationRules");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.HoursBeforeCheckIn)
                .IsRequired();

            builder.Property(x => x.RefundPercentage)
                .HasColumnType("decimal(5,2)") 
                .IsRequired();
        }
    }
}
