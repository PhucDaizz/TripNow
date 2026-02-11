using SocialService.Domain.Common;

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
    }
}
