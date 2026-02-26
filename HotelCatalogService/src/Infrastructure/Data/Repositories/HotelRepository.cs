using CloudinaryDotNet;
using HotelCatalogService.Domain.Common.Models;
using HotelCatalogService.Domain.Dto.Hotel;
using HotelCatalogService.Domain.Dto.Room;
using HotelCatalogService.Domain.Entities;
using HotelCatalogService.Domain.Enum;
using HotelCatalogService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HotelCatalogService.Infrastructure.Data.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly ApplicationDbContext _context;

        public HotelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Hotel hotel, CancellationToken cancellationToken = default)
        {
            await _context.Hotel.AddAsync(hotel, cancellationToken);
        }

        public Task UpdateAsync(Hotel hotel, CancellationToken cancellationToken = default)
        {
            _context.Hotel.Update(hotel);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Hotel hotel, CancellationToken cancellationToken = default)
        {
            _context.Hotel.Remove(hotel);
            return Task.CompletedTask;
        }

        public async Task<Hotel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Hotel.FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        }

        public async Task<Hotel?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Hotel
                .Include(h => h.Images)
                .Include(h => h.Amenities)
                .Include(h => h.RoomTypes)
                .Include(h => h.Promotions)
                .Include(h => h.Blocks)
                    .ThenInclude(b => b.Floors)
                        .ThenInclude(f => f.Rooms)
                .AsSplitQuery() 
                .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Hotel>> GetByOwnerIdAsync(Guid ownerId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Hotel
                .Where(h => h.OwnerId == ownerId)
                .OrderByDescending(h => h.CreatedAt) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking() 
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Hotel.AnyAsync(h => h.Id == id, cancellationToken);
        }

        public async Task<PagedResult<HotelDto>> GetByFilterAsync(string? searchTerm,string? city, HotelStatus? status, Guid? ownerId, decimal? minRating,
            double? userLat, double? userLon, double? radiusKm,
            decimal? minPrice, decimal? maxPrice, 
            string? sortColumn, string? sortDirection,
            int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.Hotel.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = $"%{searchTerm.Trim()}%";
                query = query.Where(h =>
                    EF.Functions.Like(h.Name, term) ||
                    EF.Functions.Like(h.Address.Street, term) ||
                    EF.Functions.Like(h.Address.City, term));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(h => h.Address.City.Contains(city.Trim()));
            }

            if (status.HasValue)
            {
                query = query.Where(h => h.Status == status.Value);
            }

            if (ownerId.HasValue)
            {
                query = query.Where(h => h.OwnerId == ownerId.Value);
            }

            if (minRating.HasValue)
            {
                query = query.Where(h => h.Rating >= minRating.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(h => h.StartingPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(h => h.StartingPrice <= maxPrice.Value);
            }

            List<HotelDto> resultItems;
            int totalCount;


            if (userLat.HasValue && userLon.HasValue && radiusKm.HasValue)
            {

                double r = radiusKm.Value / 111.0; 
                query = query.Where(h =>
                    h.Location.Latitude >= userLat.Value - r && h.Location.Latitude <= userLat.Value + r &&
                    h.Location.Longitude >= userLon.Value - r && h.Location.Longitude <= userLon.Value + r
                );

                var geoQuery = query.Select(h => new
                {
                    Hotel = h,
                    Distance = 6371 * 2 * Math.Asin(Math.Sqrt(
                        Math.Pow(Math.Sin((h.Location.Latitude * Math.PI / 180 - userLat.Value * Math.PI / 180) / 2), 2) +
                        Math.Cos(userLat.Value * Math.PI / 180) * Math.Cos(h.Location.Latitude * Math.PI / 180) *
                        Math.Pow(Math.Sin((h.Location.Longitude * Math.PI / 180 - userLon.Value * Math.PI / 180) / 2), 2)
                    ))
                })
                .Where(x => x.Distance <= radiusKm.Value); 

                totalCount = await geoQuery.CountAsync(cancellationToken);

                var data = await geoQuery
                    .OrderBy(x => x.Distance) 
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                resultItems = data.Select(x => new HotelDto
                {
                    Id = x.Hotel.Id,
                    Name = x.Hotel.Name,
                    Slug = x.Hotel.Slug,
                    Description = x.Hotel.Description,
                    OwnerId = x.Hotel.OwnerId,
                    Status = x.Hotel.Status.ToString(),
                    Rating = x.Hotel.Rating,
                    Location = x.Hotel.Location,
                    DistanceKm = Math.Round(x.Distance, 2),
                    AddressStreet = x.Hotel.Address.Street,
                    AddressCity = x.Hotel.Address.City,
                    Thumbnail = x.Hotel.Images
                        .Where(i => i.IsThumbnail)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault(),
                    CreatedAt = x.Hotel.CreatedAt
                }).ToList();
            }

            else
            {
                query = ApplySorting(query, sortColumn, sortDirection);

                totalCount = await query.CountAsync(cancellationToken);

                resultItems = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(h => new HotelDto
                    {
                        Id = h.Id,
                        Name = h.Name,
                        Slug = h.Slug,
                        Description = h.Description,
                        OwnerId = h.OwnerId,
                        Status = h.Status.ToString(),
                        Rating = h.Rating,
                        Location = h.Location,
                        DistanceKm = null,
                        AddressStreet = h.Address.Street,
                        AddressCity = h.Address.City,
                        Thumbnail = h.Images
                            .Where(i => i.IsThumbnail)
                            .Select(i => i.ImageUrl)
                            .FirstOrDefault(),
                        CreatedAt = h.CreatedAt
                    })
                    .ToListAsync(cancellationToken);
            }

            return new PagedResult<HotelDto>(resultItems, totalCount, pageNumber, pageSize);
        }

        public async Task<Hotel?> GetByIdIncludeAsync(
            Guid id,
            CancellationToken cancellationToken = default,
            params Expression<Func<Hotel, object>>[] includes)
        {
            IQueryable<Hotel> query = _context.Hotel;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query
                .AsSplitQuery()
                .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
        }

        public async Task<Hotel?> GetHotelWithRoomTypesAndImagesAsync(Guid hotelId, CancellationToken cancellationToken = default)
        {
            return await _context.Hotel
                .Include(x => x.RoomTypes)
                    .ThenInclude(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == hotelId, cancellationToken);
        }

        public async Task<Hotel?> GetHotelWithBlocksAndFloorsAsync(Guid hotelId, CancellationToken cancellationToken = default)
        {
            return await _context.Hotel
                .Include(h => h.Blocks)
                    .ThenInclude(b => b.Floors)
                .FirstOrDefaultAsync(h => h.Id == hotelId, cancellationToken);
        }

        public async Task<Hotel?> GetHotelForRoomSetupAsync(Guid hotelId, Guid blockId, Guid floorId, CancellationToken token = default)
        {
            return await _context.Hotel
                .Include(h => h.Blocks.Where(b => b.Id == blockId))
                    .ThenInclude(b => b.Floors.Where(f => f.Id == floorId))
                        .ThenInclude(f => f.Rooms)
                .FirstOrDefaultAsync(h => h.Id == hotelId, token);
        }

        public async Task<List<DirtyRoomDto>> GetDirtyRoomsAsync(Guid hotelId, Guid? blockId, Guid? floorId, CancellationToken token)
        {
            var query = _context.Hotel
                .Where(h => h.Id == hotelId)
                .SelectMany(h => h.Blocks) 
                .SelectMany(b => b.Floors, (block, floor) => new { block, floor })
                .SelectMany(x => x.floor.Rooms, (x, room) => new { x.block, x.floor, room }) 
                .Where(x => x.room.Status == RoomStatus.Dirty);

            if (blockId.HasValue)
            {
                query = query.Where(x => x.block.Id == blockId.Value);
            }

            if (floorId.HasValue)
            {
                query = query.Where(x => x.floor.Id == floorId.Value);
            }

            return await query.Select(x => new DirtyRoomDto
            {   
                RoomId = x.room.Id,
                RoomName = x.room.RoomName,
                FloorId = x.floor.Id,
                FloorName = $"Tầng {x.floor.FloorNumber}",
                BlockId = x.block.Id,
                BlockName = x.block.Name,
                Status = x.room.Status.ToString()
            }).ToListAsync(token);
        }

        public async Task<Hotel?> GetHotelWithSpecificRoomAsync(Guid hotelId, Guid roomId, CancellationToken token = default)
        {
            return await _context.Hotel
            .Include(h => h.Blocks.Where(b => b.Floors.Any(f => f.Rooms.Any(r => r.Id == roomId))))
                .ThenInclude(b => b.Floors.Where(f => f.Rooms.Any(r => r.Id == roomId)))
                    .ThenInclude(f => f.Rooms.Where(r => r.Id == roomId))
            .FirstOrDefaultAsync(h =>
                h.Id == hotelId && 
                h.Blocks.Any(b => b.Floors.Any(f => f.Rooms.Any(r => r.Id == roomId))), 
                token);

            /*return await _context.Hotel
                .Include(h => h.Blocks.Where(b => b.Floors.Any(f => f.Rooms.Any(r => r.Id == roomId))))
                    .ThenInclude(b => b.Floors.Where(f => f.Rooms.Any(r => r.Id == roomId)))
                        .ThenInclude(f => f.Rooms.Where(r => r.Id == roomId))
                .FirstOrDefaultAsync(h => h.Id == hotelId, token);*/
        }

        public async Task<Hotel?> GetHotelWithRoomTypePricesAsync(Guid hotelId, Guid roomTypeId, CancellationToken token)
        {
            return await _context.Hotel
                .Include(h => h.RoomTypes.Where(rt => rt.Id == roomTypeId))
                    .ThenInclude(rt => rt.Prices) 
                .FirstOrDefaultAsync(h => h.Id == hotelId, token);
        }

        public async Task<Hotel?> GetHotelCatalogAsync(Guid hotelId, DateTime checkInDate, CancellationToken token)
        {
            return await _context.Hotel
                .AsSplitQuery()
                .Include(h => h.RoomTypes.Where(x => x.HotelId == hotelId))
                    .ThenInclude(rt => rt.Images) 
                .Include(h => h.RoomTypes.Where(x => x.HotelId == hotelId))
                    .ThenInclude(rt => rt.Prices.Where(p => p.Date.Date == checkInDate.Date))
                .Include(h => h.RoomTypes.Where(x => x.HotelId == hotelId))
                    .ThenInclude(rt => rt.CancellationPolicy) 
                        .ThenInclude(cp => cp.Rules)
                .FirstOrDefaultAsync(h => h.Id == hotelId, token);
        }

        public async Task<Hotel?> GetHotelWithPromotionsAsync(Guid hotelId, CancellationToken token)
        {
            return await _context.Hotel
                .Include(h => h.Promotions)
                .FirstOrDefaultAsync(h => h.Id == hotelId, token);
        }

        public async Task<Hotel?> GetHotelWithSpecificPromotionAsync(Guid hotelId, string code, CancellationToken token)
        {
            var normalizedCode = code.ToUpper();

            return await _context.Hotel
                .Include(h => h.Promotions.Where(p => p.Code == normalizedCode))
                .ThenInclude(p => p.PromotionUsages) 
                .FirstOrDefaultAsync(h => h.Id == hotelId, token);
        }

        public async Task<bool> IsPromotionUsedByUserAsync(Guid hotelId, string code, Guid userId, CancellationToken token)
        {
            var normalizedCode = code.ToUpper();

            return await _context.Promotion
                .AsNoTracking()
                .AnyAsync(p =>
                    p.HotelId == hotelId &&
                    p.Code == normalizedCode &&
                    p.PromotionUsages.Any(u => u.UserId == userId),
                    token);
        }
        public async Task<Hotel?> GetHotelWithBookingPromotionUsageAsync(Guid hotelId, string code, Guid bookingId, CancellationToken token)
        {
            var normalizedCode = code.ToUpper();

            return await _context.Hotel
                .Where(h => h.Id == hotelId) 
                .Include(h => h.Promotions.Where(p => p.Code == normalizedCode)) 
                .   ThenInclude(p => p.PromotionUsages.Where(u => u.BookingId == bookingId)) 
                .FirstOrDefaultAsync(token);
        }

        private IQueryable<Hotel> ApplySorting(IQueryable<Hotel> query, string? column, string? direction)
        {
            bool isAsc = direction?.ToUpper() == "ASC";

            return column?.ToLower() switch
            {
                "name" => isAsc ? query.OrderBy(h => h.Name) : query.OrderByDescending(h => h.Name),
                "rating" => isAsc ? query.OrderBy(h => h.Rating) : query.OrderByDescending(h => h.Rating),
                "price" => isAsc ? query.OrderBy(h => h.StartingPrice) : query.OrderByDescending(h => h.StartingPrice), // Sort theo giá
                _ => query.OrderByDescending(h => h.CreatedAt) 
            };
        }

        public async Task<bool> HasRoomsInFloorAsync(Guid floorId, CancellationToken cancellationToken = default)
        {
            return await _context.Room.AsNoTracking()
                           .AnyAsync(r => r.FloorId == floorId, cancellationToken);
        }
    }
}
