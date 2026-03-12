namespace NotificationService.Application.DTOs.SystemNotification
{
    public class SystemNotificationDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } 

        public string Message { get; set; } 
        public string Type { get; set; }
        public string ReferenceId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        public static SystemNotificationDto FromEntity(Domain.Entities.Notification entity)
        {
            return new SystemNotificationDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Message = entity.Message,
                Type = entity.Type.ToString(), 
                ReferenceId = entity.ReferenceId,
                IsRead = entity.IsRead,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
