using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.HotelAmenity.Commands.RemoveHotelAmenity
{
    public class RemoveHotelAmenityCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid AmenityId { get; set; }
        public Guid OwnerId { get; set; }
    }
}
