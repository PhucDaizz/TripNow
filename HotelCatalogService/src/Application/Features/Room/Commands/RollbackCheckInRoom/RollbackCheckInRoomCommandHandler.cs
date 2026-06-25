using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Errors;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Commands.RollbackCheckInRoom
{
    public class RollbackCheckInRoomCommandHandler : IRequestHandler<RollbackCheckInRoomCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RollbackCheckInRoomCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RollbackCheckInRoomCommand request, CancellationToken cancellationToken)
        {
            var room = await _unitOfWork.Hotel.GetRoomByIdAsync(request.RoomId, cancellationToken);

            if (room == null)
            {
                return Result.Failure(RoomErrors.NotFound);
            }

            room.CancelCheckIn();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
