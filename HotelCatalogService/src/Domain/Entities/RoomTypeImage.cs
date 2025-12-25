using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Entities
{
    public class RoomTypeImage : BaseEntity
    {
        public Guid RoomTypeId { get; private set; }
        public string ImageUrl { get; private set; }
        public string PublicId { get; private set; }
        public bool IsMainImage { get; private set; } 

        private RoomTypeImage() { }

        internal RoomTypeImage(Guid roomTypeId, string imageUrl, string publicId, bool isMainImage)
        {
            RoomTypeId = roomTypeId;
            ImageUrl = imageUrl;
            PublicId = publicId;
            IsMainImage = isMainImage;
        }

        internal void SetMain(bool isMain)
        {
            IsMainImage = isMain;
        }
    }
}