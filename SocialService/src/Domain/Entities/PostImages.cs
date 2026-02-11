using SocialService.Domain.Common;
using SocialService.Domain.Exceptions; 

namespace SocialService.Domain.Entities
{
    public class PostImage : BaseEntity
    {
        public Guid PostId { get; private set; }
        public string ImageUrl { get; private set; }
        public string PublicId { get; private set; } // ID của Cloudinary (để xóa ảnh gốc)

        public int SortOrder { get; private set; }   // Thứ tự hiển thị (0, 1, 2...)
        public string? Caption { get; private set; } // Chú thích ảnh (VD: "Món này ngon lắm")

        public int Width { get; private set; }       
        public int Height { get; private set; }

        private PostImage() { } 

        internal PostImage(Guid postId, string imageUrl, string publicId, int sortOrder, int width, int height, string? caption)
        {
            if (postId == Guid.Empty) throw new DomainException("PostId không được để trống.");
            if (string.IsNullOrWhiteSpace(imageUrl)) throw new DomainException("URL ảnh không hợp lệ.");
            if (string.IsNullOrWhiteSpace(publicId)) throw new DomainException("PublicId Cloudinary bắt buộc phải có để quản lý file.");

            if (width <= 0 || height <= 0)
            {
                throw new DomainException("Kích thước ảnh (Width/Height) phải lớn hơn 0.");
            }

            PostId = postId;
            ImageUrl = imageUrl;
            PublicId = publicId;
            SortOrder = sortOrder;
            Width = width;
            Height = height;
            Caption = caption?.Trim();
        }

        /// <summary>
        /// Cập nhật thứ tự ảnh (Khi user kéo thả sắp xếp lại)
        /// </summary>
        internal void UpdateSortOrder(int newOrder)
        {
            if (newOrder < 0) throw new DomainException("Thứ tự sắp xếp không được âm.");
            SortOrder = newOrder;
        }

        /// <summary>
        /// Sửa chú thích ảnh
        /// </summary>
        internal void UpdateCaption(string? newCaption)
        {
            Caption = newCaption?.Trim();
        }
    }
}