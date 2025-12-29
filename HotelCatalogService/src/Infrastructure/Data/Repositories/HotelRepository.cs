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

        public async Task<PagedResult<HotelDto>> GetByFilterAsync(string? searchTerm, HotelStatus? status, Guid? ownerId, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
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

            if (status.HasValue)
            {
                query = query.Where(h => h.Status == status.Value);
            }

            if (ownerId.HasValue)
            {
                query = query.Where(h => h.OwnerId == ownerId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(h => h.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(h => h.CreatedAt)
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
                    AddressStreet = h.Address.Street,
                    AddressCity = h.Address.City,
                    Thumbnail = h.Images
                        .Where(i => i.IsThumbnail)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault(),
                    CreatedAt = h.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<HotelDto>(items, totalCount, pageNumber, pageSize);
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
                .FirstOrDefaultAsync(h => h.Id == hotelId, token);
        }
    }
}
