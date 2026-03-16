using HotelCatalogService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelCatalogService.Infrastructure.Data.Configurations
{
    public class CancellationPolicyConfiguration : BaseEntityConfiguration<CancellationPolicy>
    {
        public override void Configure(EntityTypeBuilder<CancellationPolicy> builder)
        {
            base.Configure(builder);

            builder.ToTable("CancellationPolicies");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Type)
               .HasConversion<byte>() 
               .HasColumnType("tinyint") 
               .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.HasMany(x => x.Rules)
                .WithOne()
                .HasForeignKey(x => x.CancellationPolicyId)
                .OnDelete(DeleteBehavior.Cascade); 
            builder.Navigation(x => x.Rules).HasField("_rules");

            builder.HasIndex(x => x.HotelId)
                   .HasDatabaseName("IX_CancellationPolicies_HotelId");
        }
    }
}
