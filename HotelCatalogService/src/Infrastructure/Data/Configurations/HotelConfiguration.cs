using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations; // Namespace chứa BaseEntityConfiguration của bạn
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

            builder.HasIndex(h => h.Slug).IsUnique();
            builder.Property(h => h.Description)
                   .HasColumnType("longtext");

            builder.Property(h => h.Rating).HasColumnType("decimal(2,1)");

            builder.Property(h => h.IsActive).IsRequired();
            builder.Property(h => h.OwnerId).IsRequired();

            // --- Value Object: Address ---
            builder.OwnsOne(h => h.Address, a =>
            {
                // Sửa: Bỏ "HasColumnType" nếu không cần thiết, EF Core tự map sang varchar
                a.Property(ad => ad.Street).HasColumnName("Address_Street").HasMaxLength(200).IsRequired();
                a.Property(ad => ad.City).HasColumnName("Address_City").HasMaxLength(100);
                a.Property(ad => ad.Country).HasColumnName("Address_Country").HasMaxLength(100);
            });

            // --- Value Object: Location ---
            builder.OwnsOne(h => h.Location, l =>
            {
                l.Property(c => c.Latitude).HasColumnName("Latitude");
                l.Property(c => c.Longitude).HasColumnName("Longitude");
            });

            // --- Relationships (Giữ nguyên) ---
            builder.HasMany(h => h.Blocks)
                   .WithOne()
                   .HasForeignKey("HotelId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(h => h.RoomTypes)
                   .WithOne()
                   .HasForeignKey("HotelId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(h => h.Amenities)
                   .WithOne()
                   .HasForeignKey("HotelId")
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(h => h.Promotions)
                   .WithOne()
                   .HasForeignKey("HotelId")
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}