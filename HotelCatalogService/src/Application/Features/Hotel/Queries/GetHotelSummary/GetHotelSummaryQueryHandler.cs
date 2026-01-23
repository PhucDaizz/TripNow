using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Hotel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Hotel.Queries.GetHotelSummary
{
    public class GetHotelSummaryQueryHandler : IRequestHandler<GetHotelSummaryQuery, Result<HotelSummaryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetHotelSummaryQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<HotelSummaryDto>> Handle(GetHotelSummaryQuery request, CancellationToken cancellationToken)
        {
            var hotel = await _context.Hotel
            .Where(x => x.Id == request.HotelId)
            .Select(x => new HotelSummaryDto
            {
                HotelName = x.Name,
                Street = x.Address.Street,
                City = x.Address.City,
                Country = x.Address.Country,
                Status = x.Status.ToString()
            })
            .FirstOrDefaultAsync(cancellationToken);

            if (hotel == null)
            {
                return Result.Failure<HotelSummaryDto>(
                    new Error("HOTEL.NOT_FOUND", "Hotel does not exist.")
                );
            }

            return Result.Success(hotel);
        }
    }
}
