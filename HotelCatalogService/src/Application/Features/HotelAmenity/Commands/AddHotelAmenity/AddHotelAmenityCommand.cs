using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.HotelAmenity.Commands.AddHotelAmenity
{
    public class AddHotelAmenityCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid OwnerId { get; set; }
        public Guid AmenityId { get; set; }

        public string? Description { get; set; }
        public bool IsFree { get; set; }
    }
}
