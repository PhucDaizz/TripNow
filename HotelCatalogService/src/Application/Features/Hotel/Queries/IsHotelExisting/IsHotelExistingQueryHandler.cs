using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Hotel.Queries.IsHotelExisting
{
    public class IsHotelExistingQueryHandler : IRequestHandler<IsHotelExistingQuery, Result<bool>>
    {
        private readonly IApplicationDbContext _context;

        public IsHotelExistingQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<bool>> Handle(IsHotelExistingQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.Hotel.AnyAsync(x => x.Id == request.HotelId, cancellationToken);
            return Result.Success(result);
        }
    }
}
