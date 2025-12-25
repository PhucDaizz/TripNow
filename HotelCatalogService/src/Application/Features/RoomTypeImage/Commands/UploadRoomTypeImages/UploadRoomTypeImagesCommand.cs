using Domain.Common.Response;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace HotelCatalogService.Application.Features.RoomTypeImage.Commands.UploadRoomTypeImages
{
    public class UploadRoomTypeImagesCommand : IRequest<Result<List<string>>>
    {
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid OwnerId { get; set; }
        public List<IFormFile> ImageFiles { get; set; }
    }
}
