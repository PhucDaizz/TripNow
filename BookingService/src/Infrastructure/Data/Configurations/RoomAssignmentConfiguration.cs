using BookingService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingService.Infrastructure.Data.Configurations
{
    public class RoomAssignmentConfiguration : BaseEntityConfiguration<RoomAssignment>
    {
        public override void Configure(EntityTypeBuilder<RoomAssignment> builder)
        {
            base.Configure(builder);

            builder.ToTable("RoomAssignments");
            builder.HasKey(ra => ra.Id);

            builder.Property(ra => ra.RoomName)
                   .HasMaxLength(100) 
                   .IsRequired();

            builder.Property(ra => ra.IsCheckedIn)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(ra => ra.CheckInTime)
                   .IsRequired(false);

            builder.Property(ra => ra.CheckOutTime)
                   .IsRequired(false);

            builder.HasIndex(ra => ra.RoomId);
        }
    }
}
