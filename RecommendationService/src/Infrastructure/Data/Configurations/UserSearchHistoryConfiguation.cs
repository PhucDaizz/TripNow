using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecommendationService.Domain.Entities;

namespace RecommendationService.Infrastructure.Data.Configurations
{
    public class UserSearchHistoryConfiguation : BaseEntityConfiguration<UserSearchHistory>
    {
        public override void Configure(EntityTypeBuilder<UserSearchHistory> builder)
        {
            base.Configure(builder);

            builder.ToTable("UserSearchHistories");

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.RawQuery)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Destination)
                .HasMaxLength(255);

            builder.Property(x => x.SearchedAt)
                .IsRequired();

            builder.HasIndex(x => new { x.UserId, x.SearchedAt });
        }
    }
}
