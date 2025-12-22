using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Entities
{
    public class HotelImage : BaseEntity
    {
        public Guid HotelId { get; private set; }
        public string ImageUrl { get; private set; }
        public bool IsThumbnail { get; private set; } 
        public string? Caption { get; private set; } 

        private HotelImage() { }

        internal HotelImage(Guid hotelId, string imageUrl, bool isThumbnail, string? caption)
        {
            HotelId = hotelId;
            ImageUrl = imageUrl;
            IsThumbnail = isThumbnail;
            Caption = caption;
        }
    }
}