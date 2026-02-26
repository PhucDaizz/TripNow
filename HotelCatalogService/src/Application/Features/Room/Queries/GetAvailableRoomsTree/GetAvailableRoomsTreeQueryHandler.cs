using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Block;
using HotelCatalogService.Application.DTOs.Floor;
using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Domain.Common;
using HotelCatalogService.Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Room.Queries.GetAvailableRoomsTree
{
    public class GetAvailableRoomsTreeQueryHandler : IRequestHandler<GetAvailableRoomsTreeQuery, Result<List<BlockResponse>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetAvailableRoomsTreeQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<BlockResponse>>> Handle(GetAvailableRoomsTreeQuery request, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (_currentUserService.Role == AppRoles.HotelOwner)
            {
                var ownerId = Guid.Parse(_currentUserService.UserId);

                var isOwner = await _context.Hotel
                    .AsNoTracking()
                    .AnyAsync(h => h.Id == request.HotelId && h.OwnerId == ownerId, cancellationToken);

                if (!isOwner)
                {
                    return Result.Failure<List<BlockResponse>>(new Error("Auth.Forbidden", "You do not own this hotel."));
                }
            }

            var listBlocks = await _context.Block
                .AsNoTracking()
                .Where(b => b.HotelId == request.HotelId)
                .Select(b => new BlockResponse
                {
                    BlockId = b.Id,
                    BlockName = b.Name,
                    Floors = b.Floors.OrderBy(f => f.FloorNumber)
                        .Select(f => new FloorResponse
                        {
                            FloorId = f.Id,
                            FloorNumber = f.FloorNumber,
                            Rooms = f.Rooms
                                .Where(r => r.RoomTypeId == request.RoomTypeId)
                                .Where(r => r.Status == RoomStatus.Available)
                                .Where(r => r.MaintenanceStart == null ||
                                            (today < r.MaintenanceStart || today > r.MaintenanceEnd))
                                .Select(r => new RoomResponse
                                {
                                    RoomId = r.Id,
                                    RoomTypeId = r.RoomTypeId,
                                    RoomName = r.RoomName,
                                    Status = r.Status.ToString()
                                })
                                .OrderBy(r => r.RoomName)
                                .ToList()
                        })
                        .Where(f => f.Rooms.Any()) 
                        .ToList()
                })
                .Where(b => b.Floors.Any()) 
                .OrderBy(b => b.BlockName)
                .ToListAsync(cancellationToken);

            return Result.Success(listBlocks);
        }
    }
}
