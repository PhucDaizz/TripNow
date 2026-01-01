using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class PromotionConfiguration : BaseEntityConfiguration<Promotion>
    {
        public override void Configure(EntityTypeBuilder<Promotion> builder)
        {
            base.Configure(builder);
            builder.ToTable("Promotions");

            builder.Property(p => p.Code).HasColumnType("varchar(20)").IsRequired();
            builder.HasIndex(p => p.Code); 

            builder.Property(p => p.DiscountValue).HasColumnType("decimal(18,2)");
            builder.Property(p => p.StartDate).IsRequired();
            builder.Property(p => p.EndDate).IsRequired();
            builder.Property(p => p.InitialQuantity).IsRequired();
            builder.Property(p => p.RemainingQuantity).IsRequired();
        }
    }
}
