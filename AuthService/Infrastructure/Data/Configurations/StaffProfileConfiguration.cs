using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class StaffProfileConfiguration : BaseEntityConfiguration<StaffProfile>
    {
        public override void Configure(EntityTypeBuilder<StaffProfile> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Position)
                .IsRequired()
                .HasMaxLength(50); 

            builder.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(450); 

            builder.Property(x => x.HotelId)
                .IsRequired();

            builder.HasOne(x => x.User)
               .WithOne(u => u.StaffProfile)
               .HasForeignKey<StaffProfile>(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UserId);  
            builder.HasIndex(x => x.HotelId); 
        }
    }
}
