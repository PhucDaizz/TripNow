namespace NotificationService.Application.DTOs.SocialNotification
{
    public class SocialNotificationDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string ActionType { get; set; }
        public string ReferenceId { get; set; }
        public string ActorAvatarUrl { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
        public string LastActorName { get; set; }
        public int ActorCount { get; set; }

        public static SocialNotificationDto FromEntity(Domain.Entities.SocialNotification entity, string avatarUrl)
        {
            string generatedMessage = entity.ActorCount > 1
                ? $"{entity.LastActorName} và {entity.ActorCount - 1} người khác đã " + GetActionText(entity.ActionType)
                : $"{entity.LastActorName} đã " + GetActionText(entity.ActionType);

            return new SocialNotificationDto
            {
                Id = entity.Id,
                Message = generatedMessage,
                ActionType = entity.ActionType.ToString(),
                ReferenceId = entity.ReferenceId,
                ActorAvatarUrl = avatarUrl, 
                IsRead = entity.IsRead,
                CreatedAt = entity.CreatedAt,
                LastActorName = entity.LastActorName,
                ActorCount = entity.ActorCount
            };
        }

        private static string GetActionText(Domain.Enum.SocialActionType type)
        {
            return type switch
            {
                Domain.Enum.SocialActionType.Like => "thích bài viết của bạn.",
                Domain.Enum.SocialActionType.Comment => "bình luận về bài viết của bạn.",
                Domain.Enum.SocialActionType.Share => "chia sẻ bài viết của bạn.",
                _ => "tương tác với bạn."
            };
        }
    }
}
