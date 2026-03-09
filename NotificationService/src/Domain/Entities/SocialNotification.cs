using NotificationService.Domain.Common;
using NotificationService.Domain.Enum;

namespace NotificationService.Domain.Entities
{
    public class SocialNotification : BaseEntity, AggregateRoot
    {
        // 1. Chủ nhân của thông báo (Người nhận)
        public Guid UserId { get; private set; }

        // 2. Phân loại MXH
        // (VD: 1 = Like, 2 = Comment, 3 = Mention, 4 = Follow)
        public SocialActionType ActionType { get; private set; }

        // 3. ID của Bài viết / Hình ảnh / Comment được tương tác
        public string ReferenceId { get; private set; }

        // 4. Người Tương Tác CUỐI CÙNG (Để hiển thị: "User B đã thích...")
        public Guid LastActorId { get; private set; }
        public string LastActorName { get; private set; }

        // 5. SỐ LƯỢNG NGƯỜI TƯƠNG TÁC (Dùng để gộp: "...và 5 người khác")
        public int ActorCount { get; private set; }

        public bool IsRead { get; private set; }
        public DateTime? ReadAt { get; private set; }

        private SocialNotification() { }

        public SocialNotification(Guid userId, SocialActionType actionType, string referenceId, Guid actorId, string actorName)
        {
            UserId = userId;
            ActionType = actionType;
            ReferenceId = referenceId;

            LastActorId = actorId;
            LastActorName = actorName;
            ActorCount = 1;
            IsRead = false;
        }

        public void AddInteraction(Guid newActorId, string newActorName)
        {
            if (LastActorId != newActorId)
            {
                LastActorId = newActorId;
                LastActorName = newActorName;
                ActorCount++;
            }

            IsRead = false;
            ReadAt = null;
        }

        public void MarkAsRead()
        {
            if (!IsRead)
            {
                IsRead = true;
                ReadAt = DateTime.UtcNow;
            }
        }
    }
}
