using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class StaffProfileRepository : IStaffProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public StaffProfileRepository(
            ApplicationDbContext context
            )
        {
            _context = context;
        }

        // Get by ID
        public async Task<StaffProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.StaffProfile
                .FirstOrDefaultAsync(sp => sp.Id == id, cancellationToken);
        }

        // Get by User ID (quan trọng nhất)
        public async Task<StaffProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.StaffProfile
                .FirstOrDefaultAsync(sp => sp.UserId == userId, cancellationToken);
        }

        // Get all
        public async Task<IReadOnlyList<StaffProfile>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.StaffProfile
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // Create
        public async Task<StaffProfile> CreateAsync(StaffProfile staffProfile, CancellationToken cancellationToken = default)
        {
            staffProfile.CreatedAt = DateTime.UtcNow;

            await _context.StaffProfile.AddAsync(staffProfile, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return staffProfile;
        }

        // Update
        public async Task<StaffProfile> UpdateAsync(StaffProfile staffProfile, CancellationToken cancellationToken = default)
        {
            staffProfile.UpdatedAt = DateTime.UtcNow;

            _context.StaffProfile.Update(staffProfile);
            await _context.SaveChangesAsync(cancellationToken);

            return staffProfile;
        }

        // Delete by ID
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var staffProfile = await GetByIdAsync(id, cancellationToken);
            if (staffProfile == null) return false;

            _context.StaffProfile.Remove(staffProfile);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        // Delete by User ID
        public async Task<bool> DeleteByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var staffProfile = await GetByUserIdAsync(userId, cancellationToken);
            if (staffProfile == null) return false;

            _context.StaffProfile.Remove(staffProfile);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        // Check exists
        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.StaffProfile
                .AnyAsync(sp => sp.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.StaffProfile
                .AnyAsync(sp => sp.UserId == userId, cancellationToken);
        }

        // Get by hotel
        public async Task<IReadOnlyList<StaffProfile>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
        {
            return await _context.StaffProfile
                .Where(sp => sp.HotelId == hotelId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // Get by position
        public async Task<IReadOnlyList<StaffProfile>> GetByPositionAsync(string position, CancellationToken cancellationToken = default)
        {
            return await _context.StaffProfile
                .Where(sp => sp.Position == position)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
