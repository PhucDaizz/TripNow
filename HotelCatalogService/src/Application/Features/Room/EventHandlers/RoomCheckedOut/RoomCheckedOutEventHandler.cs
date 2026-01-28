using HotelCatalogService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Room.EventHandlers.RoomCheckedOut
{
    public class RoomCheckedOutEventHandler : INotificationHandler<RoomCheckedOutEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationDbContext _context;

        public RoomCheckedOutEventHandler(IUnitOfWork unitOfWork, IApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task Handle(RoomCheckedOutEvent notification, CancellationToken cancellationToken)
        {
            var room = await _context.Room.FirstOrDefaultAsync(x => x.Id == notification.RoomId, cancellationToken);

            if (room == null)
            {
                return;
            }

            room.CheckOut();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
