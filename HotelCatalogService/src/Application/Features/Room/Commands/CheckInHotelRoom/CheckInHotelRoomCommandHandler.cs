using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.DTOs.Room;
using HotelCatalogService.Domain.Errors;
using HotelCatalogService.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Application.Features.Room.Commands.CheckInHotelRoom
{
    public class CheckInHotelRoomCommandHandler : IRequestHandler<CheckInHotelRoomCommand, Result<RoomResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationDbContext _context;
        public CheckInHotelRoomCommandHandler(IUnitOfWork unitOfWork, IApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<Result<RoomResponse>> Handle(CheckInHotelRoomCommand request, CancellationToken cancellationToken)
        {
            var hotel = await _unitOfWork.Hotel.GetHotelWithSpecificRoomAsync(request.HotelId, request.RoomId, cancellationToken);

            if (hotel == null)
            {
                return Result.Failure<RoomResponse>(new Error("Hotel.NotFound", "No hotel found"));
            }

            if (request.IsReceptionist)
            {
                if (request.UserTokenHotelId == null || request.UserTokenHotelId != request.HotelId)
                {
                    return Result.Failure<RoomResponse>(new Error("Auth.Forbidden", "The receptionist is only allowed to check in guests for the hotel they work at."));
                }
            }
            else
            {
                bool isOwner = hotel.OwnerId == request.CheckedInBy;
                if (!isOwner)
                {
                    return Result.Failure<RoomResponse>(new Error("Auth.Forbidden", "You are not the owner of this hotel."));
                }
            }

            var room = hotel.Blocks
                .SelectMany(b => b.Floors)
                .SelectMany(f => f.Rooms)
                .FirstOrDefault(r => r.Id == request.RoomId);

            if (room == null)
            {
                return Result.Failure<RoomResponse>(RoomErrors.NotFound);
            }

            try
            {
                room.CheckIn(request.CheckedInBy); 
            }
            catch (DomainException ex)
            {
                return Result.Failure<RoomResponse>(new Error("Room.InvalidState", ex.Message));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var roomResponse = new RoomResponse
            {
                RoomId = room.Id,
                RoomTypeId = room.RoomTypeId,
                RoomName = room.RoomName,
                Status = room.Status.ToString()
            };

            return Result.Success(roomResponse);
        }
    }
}
