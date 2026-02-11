using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialService.Domain.Entities;

namespace SocialService.Infrastructure.Data.Configurations
{
    public class LocationConfiguration : BaseEntityConfiguration<Location>
    {
        public override void Configure(EntityTypeBuilder<Location> builder)
        {
            base.Configure(builder);

            builder.ToTable("Locations");

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Address)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.Type)
                   .HasConversion<string>() 
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.AvgRating)
                   .HasColumnType("decimal(3,1)")
                   .HasDefaultValue(0);

            builder.Property(x => x.IsVerify)
                   .HasDefaultValue(false);

            builder.OwnsOne(x => x.Coordinates, c =>
            {
                c.Property(y => y.Latitude)
                 .HasColumnName("Latitude")
                 .IsRequired();

                c.Property(y => y.Longitude)
                 .HasColumnName("Longitude")
                 .IsRequired();

                c.HasIndex(y => new { y.Latitude, y.Longitude })
                 .HasDatabaseName("IX_Locations_Coordinates");
            });


            builder.HasIndex(x => x.Type)
                   .HasDatabaseName("IX_Locations_Type");

            builder.HasIndex(x => x.Name)
                   .HasDatabaseName("IX_Locations_Name");
        }
    }
}
