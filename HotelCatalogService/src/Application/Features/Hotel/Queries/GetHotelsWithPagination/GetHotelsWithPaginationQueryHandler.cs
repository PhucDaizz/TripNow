using Domain.Common.Response;
using HotelCatalogService.Application.DTOs.Hotel;
using HotelCatalogService.Domain.Common.Models;
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
                request.Status,
                request.OwnerId,
                request.IsActive,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );

            var dtos = pagedData.Items.Select(h => new HotelDto
            {
                Id = h.Id,
                Name = h.Name,
                Slug = h.Slug,
                Description = h.Description,
                OwnerId = h.OwnerId,
                Status = h.Status.ToString(), 
                Rating = h.Rating,
                AddressStreet = h.Address.Street,
                AddressCity = h.Address.City,
                Thumbnail = h.Images.FirstOrDefault(i => i.IsThumbnail)?.ImageUrl ?? "",
                CreatedAt = h.CreatedAt
            }).ToList();

            return Result.Success(new PagedResult<HotelDto>(dtos, pagedData.TotalCount, request.PageNumber, request.PageSize));
        }
    }
}
