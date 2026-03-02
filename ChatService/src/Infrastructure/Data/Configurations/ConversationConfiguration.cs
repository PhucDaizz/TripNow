using ChatService.Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatService.Infrastructure.Data.Configurations
{
    public class ConversationConfiguration : BaseEntityConfiguration<Conversations>
    {
        public override void Configure(EntityTypeBuilder<Conversations> builder)
        {
            base.Configure(builder);

            builder.ToTable("Conversations");

            builder.HasKey(c => c.Id);

            builder.HasIndex(c => new { c.UserId, c.HotelId }).IsUnique();

            builder.Property(c => c.LastMessage).HasMaxLength(500);

            builder.HasMany(c => c.MessageList)
                   .WithOne() 
                   .HasForeignKey(m => m.ConversationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(c => c.MessageList)
                   .HasField("_messages") 
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
