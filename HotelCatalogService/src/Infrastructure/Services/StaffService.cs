using HotelCatalogService.Application.Contracts;
using HotelCatalogService.Application.DTOs.StaffProfile;
using HotelCatalogService.Infrastructure.Protos;

namespace HotelCatalogService.Infrastructure.Services
{
    public class StaffService : IStaffService
    {
        private readonly StaffProfileGrpc.StaffProfileGrpcClient _grpcClient;

        public StaffService(StaffProfileGrpc.StaffProfileGrpcClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public async Task<StaffInfoDto?> GetStaffInfoAsync(string userId, CancellationToken token = default)
        {
            try
            {
                var request = new GetStaffProfileRequest();

                var response = await _grpcClient.GetStaffProfileAsync(request, cancellationToken: token);

                return new StaffInfoDto
                {
                    UserId = response.UserId,
                    HotelId = Guid.Parse(response.HotelId),
                    Position = response.Position
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
