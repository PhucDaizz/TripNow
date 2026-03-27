using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Hotel.Commands
{
    public record SyncHotelsToVectorDbCommand() : IRequest<Result>;

    public class SyncHotelsToVectorDbCommandHandler : IRequestHandler<SyncHotelsToVectorDbCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationDbContext _context;

        public SyncHotelsToVectorDbCommandHandler(
            IUnitOfWork unitOfWork, IApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<Result> Handle(SyncHotelsToVectorDbCommand request, CancellationToken cancellationToken)
        {
            var hotels = await _context.Hotel
                .Include(x => x.Amenities)
                .Include(x => x.RoomTypes)
                    .ThenInclude(rt => rt.CancellationPolicy!)
                        .ThenInclude(cp => cp.Rules)
                .Include(x => x.Images)
                .Where(x => x.Status == HotelStatus.Active)
                .ToListAsync(cancellationToken);

            foreach (var hotel in hotels)
            {
                hotel.RepublishToVectorDb();
                await _unitOfWork.Hotel.UpdateAsync(hotel, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
