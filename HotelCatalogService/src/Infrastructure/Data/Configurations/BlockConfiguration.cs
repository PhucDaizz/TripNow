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

            builder.HasIndex(p => p.HotelId)
                .HasDatabaseName("IX_Blocks_HotelId");

            builder.HasIndex(b => new { b.HotelId, b.Name })
                   .IsUnique()
                   .HasDatabaseName("IX_Blocks_HotelId_Name");

            builder.HasMany(b => b.Floors)
                   .WithOne()
                   .HasForeignKey(x => x.BlockId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(h => h.Floors).HasField("_floors");
        }
    }
}
