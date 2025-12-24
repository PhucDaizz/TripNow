using HotelCatalogService.Domain.Common;

namespace HotelCatalogService.Domain.Entities
{
    public class HotelAmenity: BaseEntity
    {
        public Guid HotelId { get; private set; } 
        public Guid AmenityId { get; private set; } 
        public string? Description { get; private set; } 
        public bool IsFree { get; private set; }

        private HotelAmenity() { }

        internal HotelAmenity(Guid hotelId, Guid amenityId, string? description, bool isFree) 
        {
            HotelId = hotelId;
            AmenityId = amenityId;
            Description = description;
            IsFree = isFree;
        }

        internal void UpdateInfo(string? description, bool isFree)
        {
            Description = description;
            IsFree = isFree;
        }
    }
}
