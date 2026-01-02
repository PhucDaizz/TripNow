using HotelCatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {

        public DbSet<Hotel> Hotel { get; set; }
        public DbSet<Amenity> Amenity { get; set; }
        public DbSet<Block> Block { get; set; }
        public DbSet<Floor> Floor { get; set; }
        public DbSet<HotelAmenity> HotelAmenity { get; set; }
        public DbSet<Promotion> Promotion { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<RoomPrice> RoomPrice { get; set; }
        public DbSet<RoomType> RoomType { get; set; }
        public DbSet<RoomTypeImage> RoomTypeImage { get; set; }
        public DbSet<HotelImage> HotelImage { get; set; }
        public DbSet<PromotionUsage> PromotionUsage { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
