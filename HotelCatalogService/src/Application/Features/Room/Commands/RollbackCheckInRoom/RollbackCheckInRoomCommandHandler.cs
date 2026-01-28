using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Room.Commands.RollbackCheckInRoom
{
    public class RollbackCheckInRoomCommandHandler : IRequestHandler<RollbackCheckInRoomCommand, Result>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public RollbackCheckInRoomCommandHandler(IApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RollbackCheckInRoomCommand request, CancellationToken cancellationToken)
        {
            var room = await _context.Room.FirstOrDefaultAsync(x => x.Id == request.RoomId, cancellationToken);

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
