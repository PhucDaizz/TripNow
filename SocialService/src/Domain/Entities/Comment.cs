using SocialService.Domain.Common;
using SocialService.Domain.Enum;
using SocialService.Domain.Events.Comment;
using SocialService.Domain.Exceptions;

namespace SocialService.Domain.Entities
{
    public class Comment: BaseEntity, AggregateRoot
    {
        public Guid PostId { get; private set; }
        public Guid AuthorId { get; private set; }
        public string Content { get; private set; }


        public bool IsDeleted { get; private set; }
        public bool IsHidden { get; private set; } // Bị admin ẩn hoặc vi phạm tiêu chuẩn
        public string? HiddenReason { get; private set; }

        public Guid? ParentCommentId { get; private set; }
        public AuthorType AuthorType { get; private set; }

        private Comment() {}

        public Comment(Guid postId, Guid userId, string content, AuthorType authorType, Guid? parentCommentId = null)
        {
            if (postId == Guid.Empty) throw new DomainException("PostId không hợp lệ.");
            if (userId == Guid.Empty) throw new DomainException("UserId không hợp lệ.");
            ValidateContent(content);

            PostId = postId;
            AuthorId = userId;
            Content = content.Trim();
            ParentCommentId = parentCommentId;

            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
            IsHidden = false;
            AuthorType = authorType;

            AddDomainEvent(new CommentCreatedEvent(this.Id, this.PostId, this.AuthorId, this.ParentCommentId));
        }

        public static Comment Create(Guid postId, Guid userId, string content, AuthorType authorType, Guid? parentCommentId = null)
        {
            return new Comment(postId, userId, content, authorType, parentCommentId);
        }

        /// <summary>
        /// Sửa nội dung comment
        /// </summary>
        public void EditContent(string newContent)
        {
            ValidateContent(newContent);

            Content = newContent.Trim();
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new CommentEditedEvent(this.Id, this.PostId, this.Content));
        }

        /// <summary>
        /// Xóa comment (Soft Delete) - Người dùng tự xóa
        /// </summary>
        public void Delete()
        {
            if (IsDeleted) return;

            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new CommentDeletedEvent(this.Id, this.PostId));
        }

        /// <summary>
        /// Admin hoặc Hệ thống ẩn comment (Moderation)
        /// </summary>
        public void HideByModerator(string reason)
        {
            IsHidden = true;
            HiddenReason = reason;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new CommentHiddenByAdminEvent(this.Id, this.PostId, reason));
        }


        private void ValidateContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new DomainException("Nội dung bình luận không được để trống.");
            }

            if (content.Length > 1000) 
            {
                throw new DomainException("Bình luận không được quá 1000 ký tự.");
            }

            // Có thể thêm check từ ngữ tục tĩu (Bad words) ở đây hoặc dùng Service riêng
        }

        public void ChangeCreateBy(Guid currentUserId)
        {
            CreatedBy = currentUserId.ToString();
        }

        public void ChangeUpdateBy(Guid currentUserId)
        {
            UpdatedBy = currentUserId.ToString();
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
