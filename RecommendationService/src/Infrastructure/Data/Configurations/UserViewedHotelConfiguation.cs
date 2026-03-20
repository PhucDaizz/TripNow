using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecommendationService.Domain.Entities;

namespace RecommendationService.Infrastructure.Data.Configurations
{
    public class UserViewedHotelConfiguation : BaseEntityConfiguration<UserViewedHotel>
    {
        public override void Configure(EntityTypeBuilder<UserViewedHotel> builder)
        {
            base.Configure(builder);

            builder.ToTable("UserViewedHotels");

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.HotelId)
                .IsRequired();

            builder.Property(x => x.ViewedAt)
                .IsRequired();

            builder.HasIndex(x => new { x.UserId, x.HotelId });
        }
    }
}
