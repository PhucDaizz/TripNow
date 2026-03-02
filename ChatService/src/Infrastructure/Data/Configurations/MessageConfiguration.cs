using ChatService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatService.Infrastructure.Data.Configurations
{
    public class MessageConfiguration : BaseEntityConfiguration<Messages>
    {
        public override void Configure(EntityTypeBuilder<Messages> builder)
        {
            base.Configure(builder);

            builder.ToTable("Messages");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Content).IsRequired();

            builder.HasIndex(m => new { m.ConversationId, m.CreatedAt })
                   .IsDescending(false, true);
        }
    }
}
