using Microsoft.EntityFrameworkCore;
using SocialService.Domain.Entities;

namespace SocialService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<SavedPost> SavedPosts { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<PostImage> PostImages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Member> Members { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
