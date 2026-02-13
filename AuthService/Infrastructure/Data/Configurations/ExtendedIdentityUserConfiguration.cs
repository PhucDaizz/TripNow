using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class ExtendedIdentityUserConfiguration : IEntityTypeConfiguration<ExtendedIdentityUser>
    {
        public void Configure(EntityTypeBuilder<ExtendedIdentityUser> builder)
        {
            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.AvatarUrl)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.Follower)
                .HasDefaultValue(0);
        }
    }
}
