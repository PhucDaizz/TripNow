using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialService.Domain.Entities;

namespace SocialService.Infrastructure.Data.Configurations
{
    public class CommentConfiguration : BaseEntityConfiguration<Comment>
    {
        public override void Configure(EntityTypeBuilder<Comment> builder)
        {
            base.Configure(builder);

            builder.ToTable("Comments");

            builder.Property(c => c.AuthorId)
                .IsRequired(true);

            builder.Property(c => c.Content)
                   .IsRequired()
                   .HasMaxLength(1000)
                   .HasColumnType("nvarchar(1000)");

            builder.Property(c => c.HiddenReason)
                   .HasMaxLength(255)
                   .IsRequired(false);

            builder.Property(c => c.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(c => c.IsHidden)
                   .HasDefaultValue(false);


            builder.HasIndex(c => c.PostId)
                   .HasDatabaseName("IX_Comments_PostId");

            builder.HasIndex(c => c.ParentCommentId)
                   .HasDatabaseName("IX_Comments_ParentCommentId");


            builder.HasOne<Post>()
                   .WithMany() 
                   .HasForeignKey(c => c.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Comment>() 
                   .WithMany()        
                   .HasForeignKey(c => c.ParentCommentId)
                   .IsRequired(false) 
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(c => !c.IsDeleted);


        }
    }
}
