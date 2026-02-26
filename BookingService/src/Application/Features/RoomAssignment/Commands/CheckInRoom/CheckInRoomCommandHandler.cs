using BookingService.Application.Common.Interfaces;
using BookingService.Application.Contracts;
using BookingService.Domain.Entities;
using BookingService.Domain.Exceptions;
using Domain.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingService.Application.Features.RoomAssignment.Commands.CheckInRoom
{
    public class CheckInRoomCommandHandler : IRequestHandler<CheckInRoomCommand, Result>
    {
        private readonly IHotelCatalogService _hotelCatalogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CheckInRoomCommandHandler> _logger;
        private readonly IHotelAuthorizationService _hotelAuthService;

        public CheckInRoomCommandHandler(
            IHotelCatalogService hotelCatalogService, 
            ICurrentUserService currentUserService, 
            IUnitOfWork unitOfWork, 
            ILogger<CheckInRoomCommandHandler> logger,
            IHotelAuthorizationService hotelAuthService)
        {
            _hotelCatalogService = hotelCatalogService;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _hotelAuthService = hotelAuthService;
        }

        public async Task<Result> Handle(CheckInRoomCommand request, CancellationToken cancellationToken)
        {
            bool hasAccess = await _hotelAuthService.HasHotelAccessAsync(request.HotelId, cancellationToken);
            if (!hasAccess)
            {
                return Result.Failure(new Error("Auth.Forbidden", "You do not have permission to perform check-in for this hotel."));
            }

            var checkInBy = Guid.Parse(_currentUserService.UserId);

            var roomInfo = await _hotelCatalogService.CheckInRoom(request.HotelId, request.RoomId, checkInBy, cancellationToken);

            if (roomInfo == null)
            {
                return Result.Failure(new Error("Room.NotAvailable", "Room not available in the Catalog."));
            }


            try
            {
                var booking = await _unitOfWork.Booking.GetBookingWithDetailItemAssignmentAsync(request.BookingId, cancellationToken);

                if (booking == null)
                {
                    await RollbackHelper(request, cancellationToken);
                    return Result.Failure(new Error("Booking.NotFound", "No reservation found."));
                }

                BookingItem targetItem = null;

                targetItem = booking.Items.FirstOrDefault(i => i.Assignments.Any(a => a.RoomId == request.RoomId));

                if (targetItem == null)
                {
                    var candidateItems = booking.Items.Where(i => i.RoomTypeId == roomInfo.RoomTypeId).ToList();
                    targetItem = candidateItems.FirstOrDefault(i => i.Assignments.Count < i.Quantity);
                }

                if (targetItem == null)
                {
                    await RollbackHelper(request, cancellationToken);
                    return Result.Failure(new Error("Mismatch", $"The room type {roomInfo.RoomName} does not match the order, or the order already has enough rooms."));
                }

                var assignment = targetItem.Assignments.FirstOrDefault(a => a.RoomId == request.RoomId);

                if (assignment == null)
                {
                    targetItem.AssignRoom(request.RoomId, roomInfo.RoomName ?? "Unknown Room");
                    assignment = targetItem.Assignments.First(a => a.RoomId == request.RoomId);
                }

                assignment.CheckIn();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (DomainException ex)
            {
                await RollbackHelper(request, cancellationToken);
                return Result.Failure(new Error("Domain.Error", ex.Message));
            }
            catch (DbUpdateConcurrencyException)
            {
                await RollbackHelper(request, cancellationToken);
                return Result.Failure(new Error("Concurrency", "The data has been modified by someone else; please try again."));
            }
            catch (Exception ex)
            {
                await RollbackHelper(request, cancellationToken);
                _logger.LogError(ex, "System error during CheckIn for Room {RoomId}", request.RoomId);
                return Result.Failure(new Error("System.Error", ex.Message));
            }
        }

        private async Task RollbackHelper(CheckInRoomCommand request, CancellationToken token)
        {
            try
            {
                _logger.LogWarning("Initiating Rollback for Room {RoomId} in Hotel {HotelId}...", request.RoomId, request.HotelId);

                await _hotelCatalogService.RollbackCheckInRoom(request.HotelId, request.RoomId, token);

                _logger.LogInformation("Rollback successful for Room {RoomId}.", request.RoomId);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "CRITICAL: ZOMBIE ROOM DETECTED! Failed to rollback Room {RoomId}. Please fix manually.", request.RoomId);
            }
        }
    }
}
