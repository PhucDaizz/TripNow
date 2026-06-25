using HotelCatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public IQueryable<Hotel> Hotels { get; }
        public IQueryable<Amenity> Amenities { get; }
        public IQueryable<Block> Blocks { get; }
        public IQueryable<Floor> Floors { get; }
        public IQueryable<HotelAmenity> HotelAmenities { get; }
        public IQueryable<Promotion> Promotions { get; }
        public IQueryable<Room> Rooms { get; }
        public IQueryable<RoomPrice> RoomPrices { get; }
        public IQueryable<RoomType> RoomTypes { get; }
        public IQueryable<RoomTypeImage> RoomTypeImages { get; }
        public IQueryable<HotelImage> HotelImages { get; }
        public IQueryable<PromotionUsage> PromotionUsages { get; }
        public IQueryable<CancellationPolicy> CancellationPolicies { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
