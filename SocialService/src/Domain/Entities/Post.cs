using SocialService.Domain.Common;
using SocialService.Domain.Enum;
using SocialService.Domain.Exceptions;

namespace SocialService.Domain.Entities
{
    public class Post : BaseEntity, AggregateRoot
    {
        public Guid UserId { get; private set; }
        public Guid? HotelId { get; private set; } // HotelId: Đánh dấu post này nằm trên tường của KS nào hoặc thuộc chuyến đi nào

        public string Content { get; private set; }
        public string? ThumbnailUrl { get; private set; }

        public PostType Type { get; private set; } // 0=Review, 1=Event, 2=Normal
        public PostStatus Status { get; private set; } // 0=Pending, 1=Active, 2=Hidden

        public int LikeCount { get; private set; }
        public int CommentCount { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        //public PostAuthorType AuthorType { get; private set; }

        public Review? ReviewDetail { get; private set; }

        private readonly List<PostImage> _images = new();
        public IReadOnlyCollection<PostImage> Images => _images.AsReadOnly();

        private Post() { }


        // 1. Tạo Post bình thường (Check-in, Khoe ảnh)
        public static Post CreateNormalPost(Guid userId, string content, Guid? hotelId = null)
        {
            return new Post(userId, hotelId, content, PostType.Normal);
        }

        // 2. Tạo Post sự kiện (Chỉ chủ khách sạn tạo)
        public static Post CreateEventPost(Guid ownerId, Guid hotelId, string content)
        {
            return new Post(ownerId, hotelId, content, PostType.Event);
        }

        // 3. Tạo Review Post 
        public static Post CreateReviewPost(Guid userId, Guid? hotelId, string content,
            TargetTypeReview targetType, Guid targetId, decimal rating, Guid? bookingId)
        {
            if (targetType == TargetTypeReview.Hotel && (hotelId == null || hotelId == Guid.Empty))
            {
                throw new DomainException("Review khách sạn bắt buộc phải gắn với một Khách sạn (HotelId không được trống).");
            }

            var post = new Post(userId, hotelId, content, PostType.Review);
            post.ReviewDetail = new Review(post.Id, targetId, targetType, rating, bookingId);

            return post;
        }

        // --- CONSTRUCTOR CHUNG ---
        private Post(Guid userId, Guid? hotelId, string content, PostType type)
        {
            if (userId == Guid.Empty) throw new DomainException("UserId không hợp lệ.");
            if (string.IsNullOrWhiteSpace(content)) throw new DomainException("Nội dung không được để trống.");

            if (type == PostType.Event && (hotelId == null || hotelId == Guid.Empty))
            {
                throw new DomainException("Bài đăng sự kiện (Event) bắt buộc phải thuộc về một Khách sạn (HotelId).");
            }

            UserId = userId;
            HotelId = hotelId == Guid.Empty ? null : hotelId;
            Content = content.Trim();
            Type = type;

            Status = PostStatus.Active;
            CreatedAt = DateTime.UtcNow;
            LikeCount = 0;
            CommentCount = 0;
            IsDeleted = false;
        }


        /// <summary>
        /// Thêm ảnh vào bài post
        /// </summary>
        public void AddImage(string url, string publicId, int width, int height, string? caption)
        {
            var newImage = new PostImage(this.Id, url, publicId, _images.Count, width, height, caption);
            _images.Add(newImage);

            if (_images.Count == 1)
            {
                ThumbnailUrl = url;
            }
        }

        /// <summary>
        /// Sắp xếp lại thứ tự ảnh
        /// </summary>
        public void ReorderImages(List<Guid> imageIdsInOrder)
        {
            // Logic tìm và cập nhật SortOrder cho từng ảnh trong _images
            // ...
        }

        /// <summary>
        /// Update số lượng like (Gọi từ Event Handler khi có PostLikedEvent)
        /// </summary>
        public void IncrementLikeCount()
        {
            LikeCount++;
        }

        public void DecrementLikeCount()
        {
            if (LikeCount > 0) LikeCount--;
        }

        public void IncrementCommentCount()
        {
            CommentCount++;
        }

        public void DecrementCommentCount()
        {
            if (CommentCount > 0) CommentCount--;
        }


        /// <summary>
        /// Xóa bài viết (Soft Delete)
        /// </summary>
        public void Delete()
        {
            IsDeleted = true;
            Status = PostStatus.Hidden;
            UpdatedAt = DateTime.UtcNow;

            // Có thể bắn event PostDeletedEvent để trừ điểm uy tín user...
        }

        public void UpdateContent(string newContent)
        {
            if (string.IsNullOrWhiteSpace(newContent)) throw new DomainException("Nội dung không được để trống.");
            Content = newContent.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveImage(Guid imageId)
        {
            var image = _images.FirstOrDefault(x => x.Id == imageId);
            if (image != null)
            {
                _images.Remove(image);
                UpdatedAt = DateTime.UtcNow;
            }
        }
        public string? GetImagePublicId(Guid imageId)
        {
            return _images.FirstOrDefault(x => x.Id == imageId)?.PublicId;
        }
    }
}