using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.RoomTypeImage;
using MediatR;

namespace HotelCatalogService.Application.Features.RoomTypeImage.Queries.GetRoomTypeImages
{
    public class GetRoomTypeImagesQuery : IRequest<Result<List<RoomTypeImageDto>>>
    {
        public Guid RoomTypeId { get; set; }
    }
}
