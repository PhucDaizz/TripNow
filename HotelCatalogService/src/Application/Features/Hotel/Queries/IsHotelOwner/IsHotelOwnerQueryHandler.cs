using HotelCatalogService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Hotel.Queries.IsHotelOwner
{
    public class IsHotelOwnerQueryHandler : IRequestHandler<IsHotelOwnerQuery, bool>
    {
        private readonly IApplicationDbContext _context;

        public IsHotelOwnerQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(IsHotelOwnerQuery request, CancellationToken cancellationToken)
        {
            var hotel = await _context.Hotel
                .Where(h => h.Id == request.HotelId && h.OwnerId == request.UserId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (hotel == null) 
                return false;

            return true;
        }
    }
}
