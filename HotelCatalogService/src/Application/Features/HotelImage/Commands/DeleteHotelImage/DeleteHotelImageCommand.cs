using Domain.Common.Response;
using MediatR;

namespace HotelCatalogService.Application.Features.HotelImage.Commands.DeleteHotelImage
{
    public class DeleteHotelImageCommand : IRequest<Result>
    {
        public Guid HotelId { get; set; }
        public Guid ImageId { get; set; }
        public Guid OwnerId { get; set; } 
    }
}
