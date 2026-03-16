using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class RoomTypeConfiguration : BaseEntityConfiguration<RoomType>
    {
        public override void Configure(EntityTypeBuilder<RoomType> builder)
        {
            base.Configure(builder);
            builder.ToTable("RoomTypes");

            builder.Property(rt => rt.Name).HasMaxLength(150).IsRequired();
            builder.Property(rt => rt.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(rt => rt.SizeM2).HasColumnType("decimal(4,2)");
            builder.Property(rt => rt.Capacity).IsRequired();

            builder.HasMany(rt => rt.Prices)
                   .WithOne()
                   .HasForeignKey(x => x.RoomTypeId) 
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(h => h.Prices).HasField("_prices");


            builder.HasMany(rt => rt.Images)
                   .WithOne()
                   .HasForeignKey(x => x.RoomTypeId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(h => h.Images).HasField("_images");

            builder.HasMany(rt => rt.Rooms)
                   .WithOne()
                   .HasForeignKey(x => x.RoomTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.Navigation(h => h.Rooms).HasField("_rooms");

            builder.HasOne(rt => rt.CancellationPolicy)
                   .WithMany() 
                   .HasForeignKey(rt => rt.CancellationPolicyId)
                   .IsRequired(false) 
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(rt => rt.HotelId)
                   .HasDatabaseName("IX_RoomTypes_HotelId");
        }
    }
}
