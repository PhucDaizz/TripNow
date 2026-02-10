using BookingService.Application.Common.Interfaces;
using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.Inventory.Queries.CheckRoomUsage
{
    public class CheckRoomUsageQueryHandler : IRequestHandler<CheckRoomUsageQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckRoomUsageQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CheckRoomUsageQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Booking.HaveAnyBookInFuture(request.RoomTypeId, cancellationToken);
            if (result)
            {
                return result;
            }
            return result;
        }
    }
}
