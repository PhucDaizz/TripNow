using Domain.Common.Response;
using HotelCatalogService.Domain.Common.Models;
using HotelCatalogService.Domain.Dto.Hotel;
using HotelCatalogService.Domain.Repositories;
using MediatR;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelsWithPagination
{
    public class GetHotelsWithPaginationQueryHandler : IRequestHandler<GetHotelsWithPaginationQuery, Result<PagedResult<HotelDto>>>
    {
        private readonly IHotelRepository _hotelRepository;

        public GetHotelsWithPaginationQueryHandler(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task<Result<PagedResult<HotelDto>>> Handle(GetHotelsWithPaginationQuery request, CancellationToken cancellationToken)
        {
            var pagedData = await _hotelRepository.GetByFilterAsync(
                request.SearchTerm,
                request.City,
                request.Status,
                request.OwnerId,
                request.MinRating,
                request.UserLatitude,
                request.UserLongitude,
                request.RadiusInKm,
                request.MinPrice, 
                request.MaxPrice,
                request.SortColumn,
                request.SortDirection,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );

            
            return Result.Success(pagedData);
        }
    }
}
