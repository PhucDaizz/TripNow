using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Commands.UpdateHotelImage
{
    public class UpdateHotelImageCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid ImageId { get; set; }
        public Guid OwnerId { get; set; }
        public bool IsThumbnail { get; set; }
        public string? Caption { get; set; }
    }
}
