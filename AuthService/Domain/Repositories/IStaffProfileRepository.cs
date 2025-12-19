using Domain.Entities;

namespace Domain.Repositories
{
    public interface IStaffProfileRepository
    {
        Task<StaffProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<StaffProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<StaffProfile>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<StaffProfile> CreateAsync(StaffProfile staffProfile, CancellationToken cancellationToken = default);
        Task<StaffProfile> UpdateAsync(StaffProfile staffProfile, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<StaffProfile>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<StaffProfile>> GetByPositionAsync(string position, CancellationToken cancellationToken = default);
    }
}
