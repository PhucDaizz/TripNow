using HotelCatalogService.Domain.Common.Models;
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

        public async Task<PagedResult<Hotel>> GetByFilterAsync(string? searchTerm, HotelStatus? status, Guid? ownerId, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.Hotel.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(h => h.Name.ToLower().Contains(term) ||
                                         h.Address.Street.ToLower().Contains(term) ||
                                         h.Address.City.ToLower().Contains(term));
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

            query = query.OrderByDescending(h => h.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Hotel>(items, totalCount, pageNumber, pageSize);
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

        public async Task<Hotel> GetHotelWithRoomTypesAndImagesAsync(Guid hotelId, CancellationToken cancellationToken = default)
        {
            return await _context.Hotel
                .Include(x => x.RoomTypes)
                    .ThenInclude(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == hotelId, cancellationToken);
        }

        public async Task<Hotel> GetHotelWithBlocksAndFloorsAsync(Guid hotelId, CancellationToken cancellationToken = default)
        {
            return await _context.Hotel
                .Include(h => h.Blocks)
                    .ThenInclude(b => b.Floors)
                .FirstOrDefaultAsync(h => h.Id == hotelId, cancellationToken);
        }
    }
}
