using Microsoft.EntityFrameworkCore;
using SocialService.Domain.Entities;

namespace SocialService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        IQueryable<Comment> Comments { get; }
        IQueryable<PostLike> PostLikes { get; }
        IQueryable<Location> Locations { get; }
        IQueryable<SavedPost> SavedPosts { get; }
        IQueryable<UserFollow> UserFollows { get; }
        IQueryable<Review> Reviews { get; }
        IQueryable<PostImage> PostImages { get; }
        IQueryable<Post> Posts { get; }
        IQueryable<Member> Members { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
