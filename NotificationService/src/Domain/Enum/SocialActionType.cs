namespace NotificationService.Domain.Enum
{
    public enum SocialActionType : byte
    {
        Like = 1,           // Thích/Thả tim bài viết, hình ảnh, review
        Comment = 2,        // Bình luận vào bài viết
        ReplyComment = 3,   // Trả lời bình luận của bác
        Mention = 4,        // Tag/Nhắc tên bác (@username) trong bài/comment
        Follow = 5,         // Bắt đầu theo dõi (Khách theo dõi Khách sạn)
        Share = 6           // Chia sẻ bài viết của bác
    }
}
