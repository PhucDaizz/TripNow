using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class HotelConfiguration : BaseEntityConfiguration<Hotel>
    {
        public override void Configure(EntityTypeBuilder<Hotel> builder)
        {
            base.Configure(builder);

            builder.ToTable("Hotels");

            builder.Property(h => h.Name).HasMaxLength(250).IsRequired();

            builder.Property(h => h.Slug)
                   .HasMaxLength(300) 
                   .IsRequired();

            builder.Property(h => h.StartingPrice)
                    .HasColumnType("decimal(18,2)").IsRequired();

            builder.HasIndex(h => h.Slug).IsUnique();
            builder.Property(h => h.Description)
                   .HasColumnType("longtext");

            builder.Property(h => h.Rating).HasColumnType("decimal(2,1)");

            builder.Property(h => h.OwnerId).IsRequired();

            // --- Value Object: Address ---
            builder.OwnsOne(h => h.Address, a =>
            {
                // Sửa: Bỏ "HasColumnType" nếu không cần thiết, EF Core tự map sang varchar
                a.Property(ad => ad.Street).HasColumnName("Address_Street").HasMaxLength(200).IsRequired();
                a.Property(ad => ad.City).HasColumnName("Address_City").HasMaxLength(100);
                a.Property(ad => ad.Country).HasColumnName("Address_Country").HasMaxLength(100);

                a.HasIndex(ad => ad.City)
                    .HasDatabaseName("IX_Hotels_Address_City");
            });

            // --- Value Object: Location ---
            builder.OwnsOne(h => h.Location, l =>
            {
                l.Property(c => c.Latitude).HasColumnName("Latitude");
                l.Property(c => c.Longitude).HasColumnName("Longitude");

                l.HasIndex(c => new { c.Latitude, c.Longitude })
                    .HasDatabaseName("IX_Hotels_Location");
            });

            // --- Relationships (Giữ nguyên) ---
            builder.HasMany(h => h.Blocks)
                   .WithOne()
                   .HasForeignKey(x => x.HotelId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(h => h.Blocks).HasField("_blocks");

            builder.HasMany(h => h.RoomTypes)
                   .WithOne()
                   .HasForeignKey(x => x.HotelId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(h => h.RoomTypes).HasField("_roomTypes");

            builder.HasMany(h => h.Amenities)
                   .WithOne()
                   .HasForeignKey(x => x.HotelId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(h => h.Amenities).HasField("_amenities");

            builder.HasMany(h => h.Promotions)
                   .WithOne()
                   .HasForeignKey(x => x.HotelId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(h => h.Promotions).HasField("_promotions");

            builder.HasMany(h => h.Images) 
                   .WithOne()
                   .HasForeignKey(x => x.HotelId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(h => h.Images).HasField("_images");

            builder.HasIndex(h => new { h.Status, h.Rating })
            .HasDatabaseName("IX_Hotels_Status_Rating");

            builder.HasIndex(h => new { h.Status, h.StartingPrice })
                   .HasDatabaseName("IX_Hotels_Status_Price");

            builder.HasIndex(h => h.OwnerId)
                .HasDatabaseName("IX_Hotels_OwnerId");

            builder.HasIndex(h => h.Name);
        }
    }
}