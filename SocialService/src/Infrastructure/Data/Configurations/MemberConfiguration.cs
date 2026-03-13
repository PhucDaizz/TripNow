using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
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

            builder.Property(x => x.Type)
                   .HasColumnType("tinyint")
                   .IsRequired();

            builder.HasIndex(x => x.FullName)
                   .HasDatabaseName("IX_Members_FullName");

            builder.HasIndex(x => x.Type)
                   .HasDatabaseName("IX_Members_Type");

            builder.HasIndex(x => new { x.Type, x.FullName })
                   .HasDatabaseName("IX_Members_Type_FullName");
        }
    }
}
