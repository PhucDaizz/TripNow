using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Nexus.BuildingBlocks.Model;
using SocialService.Application.Contracts;
using SocialService.Application.DTOs.Hotel;
using SocialService.Infrastructure.Protos;
using System.Net.Http.Json;

namespace SocialService.Infrastructure.Services
{
    public class HotelCatalogService : IHotelCatalogService
    {
        private readonly CatalogGrpc.CatalogGrpcClient _grpcClient;

        public HotelCatalogService(CatalogGrpc.CatalogGrpcClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public async Task<bool> IsHotelExisting(Guid hotelId, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new IsHotelExistingRequest { HotelId = hotelId.ToString() };
                var response = await _grpcClient.IsHotelExistingAsync(request, cancellationToken: cancellationToken);
                return response.IsExisting;
            }
            catch (RpcException)
            {
                return false;
            }
        }

        public async Task<HotelDetailDto?> GetHotelDetail(Guid hotelId, CancellationToken cancellation = default)
        {
            try
            {
                var request = new GetHotelDetailRequest { HotelId = hotelId.ToString() };
                var response = await _grpcClient.GetHotelDetailAsync(request, cancellationToken: cancellation);

                return new HotelDetailDto
                {
                    Id = Guid.Parse(response.Id),
                    OwnerId = Guid.Parse(response.OwnerId),
                    Name = response.Name,
                    Follower = response.Follower,
                    Slug = response.Slug,
                    Description = response.Description,
                    AddressStreet = response.AddressStreet,
                    AddressCity = response.AddressCity,
                    Status = response.Status,
                    Rating = (decimal)response.Rating,
                    Location = response.Location != null
                        ? new Domain.ValueObject.Coordinates(response.Location.Latitude, response.Location.Longitude)
                        : null,
                    DistanceKm = response.DistanceKm,
                    Thumbnail = response.Thumbnail
                };
            }
            catch (RpcException)
            {
                return null;
            }
        }
    }
}
