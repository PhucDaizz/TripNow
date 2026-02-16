using SocialService.Domain.Common;
using SocialService.Domain.Events.PostLike;

namespace SocialService.Domain.Entities
{
    public class PostLike: BaseEntity, AggregateRoot
    {
        public Guid PostId { get; private set; }
        public Guid UserId { get; private set; }

        private PostLike() { }

        internal PostLike(Guid postId, Guid userId)
        {
            PostId = postId;
            UserId = userId;
        }

        public static PostLike Create(Guid postId, Guid userId)
        {
            var like = new PostLike(postId, userId);

            like.AddDomainEvent(new PostLikedEvent(postId));

            return like;
        }

        public void Delete()
        {
            this.AddDomainEvent(new PostUnlikedEvent(this.PostId));
        }
    }
}
