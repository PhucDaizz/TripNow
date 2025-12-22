using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class BlockConfiguration : BaseEntityConfiguration<Block>
    {
        public override void Configure(EntityTypeBuilder<Block> builder)
        {
            base.Configure(builder);

            builder.ToTable("Blocks");

            builder.Property(b => b.Name).HasMaxLength(50).IsRequired();

            builder.HasMany(b => b.Floors)
                   .WithOne()
                   .HasForeignKey("BlockId")
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
