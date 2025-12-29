using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.Contracts;
using HotelCatalogService.Domain.Common;
using HotelCatalogService.Domain.Enum;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Commands.UpdateRoomStatus
{
    public class UpdateRoomStatusCommandHandler : IRequestHandler<UpdateRoomStatusCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStaffService _staffService;

        public UpdateRoomStatusCommandHandler(IUnitOfWork unitOfWork, IStaffService staffService)
        {
            _unitOfWork = unitOfWork;
            _staffService = staffService;
        }

        public async Task<Result> Handle(UpdateRoomStatusCommand request, CancellationToken token)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelForRoomSetupAsync(request.HotelId, request.BlockId, request.FloorId, token);
            
            if (hotel == null) return Result.Failure<Guid>(new Error("Hotel.NotFound", "Not found"));

            var block = hotel.Blocks.FirstOrDefault();
            if (block == null) return Result.Failure<Guid>(new Error("Block.NotFound", "Block does not match or does not exist."));

            var floor = block.Floors.FirstOrDefault();
            if (floor == null) return Result.Failure<Guid>(new Error("Floor.NotFound", "Floor does not match or does not exist."));


            bool isAllowed = false;
            if (hotel.OwnerId == request.OwnerId)
            {
                isAllowed = true;
            }
            else
            {
                var staffInfo = await _staffService.GetStaffInfoAsync(request.OwnerId.ToString(), token);

                if (staffInfo != null)
                {
                    bool isCorrectHotel = staffInfo.HotelId == request.HotelId;

                    bool isAllowedPosition =
                        staffInfo.Position == AppRoles.Housekeeping ||
                        staffInfo.Position == AppRoles.Receptionist;

                    if (isCorrectHotel && isAllowedPosition)
                    {
                        isAllowed = true;
                    }
                }
            }

            if (!isAllowed)
            {
                return Result.Failure(new Error("Forbidden", "You do not have permission to update the status of this room."));
            }


            var room = hotel?.Blocks.FirstOrDefault(b => b.Id == request.BlockId)
                              ?.Floors.FirstOrDefault(f => f.Id == request.FloorId)
                              ?.Rooms.FirstOrDefault(r => r.Id == request.RoomId);

            if (room == null) return Result.Failure(new Error("Room.NotFound", "Room not found"));

            try
            {
                switch (request.NewStatus)
                {
                    case RoomStatus.Dirty:
                        room.MarkAsDirty();
                        break;
                    case RoomStatus.Maintain:
                        room.MarkAsMaintain();
                        break;
                    case RoomStatus.Cleaning:
                        room.StartCleaning();
                        break;
                    case RoomStatus.Available:
                        room.FinishCleaning();
                        break;
                    default:
                        return Result.Failure(new Error("Status.Invalid", "Invalid status"));
                }

                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);
                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(new Error("Room.LogicError", ex.Message));
            }
        }
    }
}
