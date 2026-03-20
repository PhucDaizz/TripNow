using RecommendationService.Domain.Entities;

namespace RecommendationService.Domain.Repositories
{
    public interface IUserViewedHotelRepository
    {
        Task AddAsync(UserViewedHotel entity);
        Task<UserViewedHotel?> GetByUserAndHotelAsync(Guid userId, Guid hotelId);
        Task<IEnumerable<UserViewedHotel>> GetByUserIdAsync(Guid userId, int limit = 10);
        void Update(UserViewedHotel entity);
    }
}
