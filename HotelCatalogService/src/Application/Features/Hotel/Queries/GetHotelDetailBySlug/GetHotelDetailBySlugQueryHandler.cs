using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelDetailBySlug
{
    public class GetHotelDetailBySlugQueryHandler : IRequestHandler<GetHotelDetailBySlugQuery, Result<HotelDetailDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IIntegrationEventService _integrationEventService;
        private readonly ICurrentUserService _currentUserService;

        public GetHotelDetailBySlugQueryHandler(
            IApplicationDbContext context, 
            IIntegrationEventService integrationEventService,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _integrationEventService = integrationEventService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<HotelDetailDto>> Handle(GetHotelDetailBySlugQuery request, CancellationToken token)
        {
            var hotel = await _context.Hotels
                .AsNoTracking()
                .Include(x => x.Images)
                .FirstOrDefaultAsync(h => h.Slug == request.Slug, token);

            if (hotel == null)
                return Result.Failure<HotelDetailDto>(new Error("Hotel.NotFound", "Can not found this hotel."));

            var dto = new HotelDetailDto
            {
                Id = hotel.Id,
                OwnerId = hotel.OwnerId,
                Name = hotel.Name,
                Follower = hotel.Follower,
                Slug = hotel.Slug,
                Description = hotel.Description,
                AddressStreet = hotel.Address.Street,
                AddressCity = hotel.Address.City,
                Status = hotel.Status.ToString(),
                Rating = hotel.Rating,
                Location = hotel.Location,
                DistanceKm = null,
                Thumbnail = hotel.Images
                        .Where(i => i.IsThumbnail)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
            };

            var userIdStr = _currentUserService.UserId;
            if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out Guid userId))
            {
                var viewEvent = new UserViewedHotelIntegrationEvent
                {
                    UserId = userId,
                    HotelId = hotel.Id,
                    ViewedAt = DateTime.UtcNow
                };

                await _integrationEventService.PublishAsync(
                    viewEvent,
                    exchange: "hotel-catalog.events",
                    exchangeType: "topic",
                    routingKey: "hotel.viewed",
                    cancellationToken: token
                );
            }

            return Result.Success(dto);
        }
    }
}
