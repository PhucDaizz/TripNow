using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.HotelImage;
using MediatR;

namespace HotelCatalogService.Application.Features.HotelImage.Queries
{
    public class GetHotelImagesQuery : IRequest<Result<List<HotelImageDto>>>
    {
        public Guid HotelId { get; set; }
    }
}
