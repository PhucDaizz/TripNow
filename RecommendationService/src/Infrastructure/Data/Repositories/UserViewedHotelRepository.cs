using Microsoft.EntityFrameworkCore;
using RecommendationService.Domain.Entities;
using RecommendationService.Domain.Repositories;

namespace RecommendationService.Infrastructure.Data.Repositories
{
    public class UserViewedHotelRepository : IUserViewedHotelRepository
    {
        private readonly ApplicationDbContext _context;

        public UserViewedHotelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserViewedHotel entity)
        {
            await _context.UserViewedHotels.AddAsync(entity);
        }

        public async Task<UserViewedHotel?> GetByUserAndHotelAsync(Guid userId, Guid hotelId)
        {
            return await _context.UserViewedHotels
                .FirstOrDefaultAsync(x => x.UserId == userId && x.HotelId == hotelId);
        }

        public async Task<IEnumerable<UserViewedHotel>> GetByUserIdAsync(Guid userId, int limit = 10)
        {
            return await _context.UserViewedHotels
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.ViewedAt)
                .Take(limit)
                .ToListAsync();
        }

        public void Update(UserViewedHotel entity)
        {
            _context.UserViewedHotels.Update(entity);
        }
    }
}
