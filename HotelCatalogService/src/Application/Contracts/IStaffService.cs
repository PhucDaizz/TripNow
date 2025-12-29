using HotelCatalogService.Application.DTOs.StaffProfile;

namespace HotelCatalogService.Application.Contracts
{
    public interface IStaffService
    {
        Task<StaffInfoDto?> GetStaffInfoAsync(string userId, CancellationToken token = default);
    }
}
