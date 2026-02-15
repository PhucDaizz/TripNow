using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialService.Domain.Entities;

namespace SocialService.Infrastructure.Data.Configurations
{
    public class MemberConfiguration : BaseEntityConfiguration<Member>
    {
        public override void Configure(EntityTypeBuilder<Member> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.FullName).HasMaxLength(100);
            builder.Property(x => x.AvatarUrl).HasMaxLength(500);
        }
    }
}
