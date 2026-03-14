using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.Contracts;
using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Domain.Common;
using HotelCatalogService.Domain.Enum;
using MediatR;

namespace HotelCatalogService.Application.Features.Room.Commands.UpdateRoomStatus
{
    public class UpdateRoomStatusCommandHandler : IRequestHandler<UpdateRoomStatusCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStaffService _staffService;
        private readonly IHousekeepingSignalRService _signalRService;
        private readonly ICurrentUserService _currentUserService;

        public UpdateRoomStatusCommandHandler(
            IUnitOfWork unitOfWork, 
            IStaffService staffService,
            IHousekeepingSignalRService signalRService,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _staffService = staffService;
            _signalRService = signalRService;
            _currentUserService = currentUserService;
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
            string staffName = _currentUserService.FullName ?? "Người dùng ẩn danh";

            if (hotel.OwnerId == request.OwnerId)
            {
                isAllowed = true;
                if (string.IsNullOrEmpty(_currentUserService.FullName))
                    staffName = "Chủ khách sạn";
            }
            else
            {
                bool isCorrectHotel = _currentUserService.HotelId == request.HotelId;
                bool isAllowedPosition = (_currentUserService.Role == AppRoles.Housekeeping) ||
                                         (_currentUserService.Role == AppRoles.Receptionist);

                if (isCorrectHotel && isAllowedPosition)
                {
                    isAllowed = true;
                }
                else
                {
                    var staffInfo = await _staffService.GetStaffInfoAsync(request.OwnerId.ToString(), token);
                    if (staffInfo != null && staffInfo.HotelId == request.HotelId &&
                       (staffInfo.Position == AppRoles.Housekeeping || staffInfo.Position == AppRoles.Receptionist))
                    {
                        isAllowed = true;
                        staffName = "Nhân viên dọn phòng";
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
                Guid actionUserId = Guid.Parse(_currentUserService.UserId!);

                switch (request.NewStatus)
                {
                    case RoomStatus.Dirty:
                        room.MarkAsDirty();
                        break;
                    case RoomStatus.Cleaning:
                        room.StartCleaning(actionUserId);
                        break;
                    case RoomStatus.Available:
                        room.FinishCleaning(actionUserId);
                        break;
                    default:
                        return Result.Failure(new Error("Status.Invalid", "Invalid status"));
                }

                await _unitOfWork.Hotel.UpdateAsync(hotel, token);
                await _unitOfWork.SaveChangesAsync(token);

                var updateDto = new RoomStatusUpdateDto
                {
                    RoomId = room.Id,
                    RoomName = room.RoomName ?? "Phòng",
                    BlockId = block.Id,
                    BlockName = block.Name,
                    FloorId = floor.Id,
                    FloorName = floor.FloorNumber.ToString(),
                    Status = request.NewStatus.ToString(),
                    AssignedToStaffId = request.NewStatus == RoomStatus.Cleaning ? actionUserId : null,
                    AssignedToStaffName = request.NewStatus == RoomStatus.Cleaning ? staffName : null
                };

                // Phát thanh cho tất cả nhân viên dọn phòng của KS này
                await _signalRService.BroadcastRoomStatusAsync(request.HotelId, updateDto);

                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(new Error("Room.LogicError", ex.Message));
            }
        }
    }
}
