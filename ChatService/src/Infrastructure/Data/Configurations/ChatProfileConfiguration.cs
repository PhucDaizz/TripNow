using ChatService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatService.Infrastructure.Data.Configurations
{
    public class ChatProfileConfiguration : BaseEntityConfiguration<ChatProfile>
    {
        public override void Configure(EntityTypeBuilder<ChatProfile> builder)
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
