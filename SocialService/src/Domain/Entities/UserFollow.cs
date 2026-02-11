using SocialService.Domain.Common;
using SocialService.Domain.Enum;
using SocialService.Domain.Exceptions; 

namespace SocialService.Domain.Entities
{
    public class UserFollow : BaseEntity, AggregateRoot
    {
        public Guid FollowerId { get; private set; }
        public Guid TargetId { get; private set; } 
        public TypeFollow Type { get; private set; }


        private UserFollow() { } 
        public UserFollow(Guid followerId, Guid targetId, TypeFollow type)
        {
            if (followerId == Guid.Empty) throw new DomainException("Người follow (FollowerId) không hợp lệ.");
            if (targetId == Guid.Empty) throw new DomainException("Đối tượng được follow (TargetId) không hợp lệ.");

            if (type == TypeFollow.FollowUser && followerId == targetId)
            {
                throw new DomainException("Bạn không thể tự theo dõi chính mình.");
            }

            FollowerId = followerId;
            TargetId = targetId;
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }

    }
}