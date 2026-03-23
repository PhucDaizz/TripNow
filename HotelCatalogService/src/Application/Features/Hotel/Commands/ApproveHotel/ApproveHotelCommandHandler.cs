using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Hotel.Commands.ApproveHotel
{
    public class ApproveHotelCommandHandler : IRequestHandler<ApproveHotelCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationDbContext _context;

        public ApproveHotelCommandHandler(
            IUnitOfWork unitOfWork, IApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<Result> Handle(ApproveHotelCommand request, CancellationToken cancellationToken)
        {
            var hotel = await _context.Hotel
                .Include(x => x.Amenities)
                .Include(x => x.RoomTypes)
                    .ThenInclude(rt => rt.CancellationPolicy!)
                        .ThenInclude(cp => cp.Rules)
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == request.HotelId);

            if (hotel == null)
            {
                return Result.Failure(new Error("Hotel.NotFound", "Không tìm thấy khách sạn"));
            }

            hotel.Approve();

            await _unitOfWork.Hotel.UpdateAsync(hotel, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
