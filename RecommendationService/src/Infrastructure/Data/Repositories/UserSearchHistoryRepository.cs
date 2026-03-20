using Microsoft.EntityFrameworkCore;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Domain.Entities;
using RecommendationService.Domain.Repositories;

namespace RecommendationService.Infrastructure.Data.Repositories
{
    public class UserSearchHistoryRepository : IUserSearchHistoryRepository
    {
        private readonly IApplicationDbContext _context;

        public UserSearchHistoryRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserSearchHistory entity)
        {
            await _context.UserSearchHistories.AddAsync(entity);
        }

        public async Task<UserSearchHistory?> GetByIdAsync(Guid id)
        {
            return await _context.UserSearchHistories.FindAsync(id);
        }

        public async Task<IEnumerable<UserSearchHistory>> GetByUserIdAsync(Guid userId, int limit = 10)
        {
            return await _context.UserSearchHistories
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.SearchedAt)
                .Take(limit)
                .ToListAsync();
        }
    }
}
