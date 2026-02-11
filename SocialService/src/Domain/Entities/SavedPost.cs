using SocialService.Domain.Common;

namespace SocialService.Domain.Entities
{
    public class SavedPost: BaseEntity, AggregateRoot
    {
        public Guid PostId { get; private set; }
        public Guid UserId { get; private set; }

        private SavedPost() { }

        public SavedPost(Guid postId, Guid userId)
        {
            PostId = postId;
            UserId = userId;
        }
    }
}
