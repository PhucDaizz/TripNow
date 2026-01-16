using BookingService.Application.Common.Interfaces;
using BookingService.Application.Contracts;
using BookingService.Application.DTOs.HotelCatalog;
using BookingService.Domain.Exceptions;
using MediatR;

namespace BookingService.Application.Features.Booking.Commands.CreateBooking;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHotelCatalogService _hotelCatalogService;
    private readonly IIntegrationEventService _integrationEventService;

    public CreateBookingCommandHandler(ICurrentUserService currentUserService, 
        IUnitOfWork unitOfWork, 
        IHotelCatalogService hotelCatalogService,
        IIntegrationEventService integrationEventService)
    {
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _hotelCatalogService = hotelCatalogService;
        _integrationEventService = integrationEventService;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId == null || !Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated or invalid user ID.");
        }

        var booking = new Domain.Entities.Booking(
            userId,
            request.HotelId,
            request.CheckInDate,
            request.CheckOutDate,
            request.Source,
            Guid.Parse(_currentUserService.UserId)
        );

        var allRoomPrices = await _hotelCatalogService.GetBatchRoomPricesAsync(
            request.HotelId,
            request.CheckInDate,
            request.CheckOutDate,
            cancellationToken
        );

        if (allRoomPrices == null || !allRoomPrices.Any())
        {
            throw new DomainException("Unable to retrieve the room rate list from the system.");
        }

        decimal tempBaseTotal = 0;
        int totalNights = request.CheckOutDate.DayNumber - request.CheckInDate.DayNumber;

        if (totalNights <= 0) throw new DomainException("The check-out date must be after the check-in date.");

        foreach (var item in request.Items)
        {
            var roomPriceData = allRoomPrices.FirstOrDefault(x => x.RoomTypeId == item.RoomTypeId);

            if (roomPriceData == null)
            {
                throw new DomainException($"Room type {item.RoomTypeId} does not exist or has no price yet.");
            }

            decimal totalRoomCost = 0;

            for (var date = request.CheckInDate; date < request.CheckOutDate; date = date.AddDays(1))
            {
                var dailyPrice = roomPriceData.Calendar
                    .FirstOrDefault(c => DateOnly.FromDateTime(c.Date) == date);

                decimal priceForNight = dailyPrice != null ? dailyPrice.Price : roomPriceData.BasePrice;

                totalRoomCost += priceForNight;
            }

            decimal avgUnitPrice = totalRoomCost / totalNights;

            booking.AddItem(item.RoomTypeId, item.Quantity, avgUnitPrice);
            tempBaseTotal += (totalRoomCost * item.Quantity);
        }

        if (!string.IsNullOrWhiteSpace(request.PromotionCode) && request.PromotionId.HasValue)
        {
            var applyResult = await _hotelCatalogService.ApplyPromotionAsync(
                request.HotelId,
                request.PromotionCode,
                tempBaseTotal,
                userId,
                booking.Id, 
                cancellationToken
            );

            if (!applyResult.IsSuccess)
            {
                throw new DomainException($"Discount code cannot be applied.: {applyResult.Message}");
            }

            booking.ApplyPromotion(
                request.PromotionId.Value,
                request.PromotionCode,
                applyResult.DiscountAmount 
            );
        }

        try
        {
            await _unitOfWork.Booking.AddBookingAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _integrationEventService.PublishAsync<BookingCancelledEvent>(
                new BookingCancelledEvent
                {
                    HotelId = request.HotelId,
                    BookingId = booking.Id,
                    PromotionCode = request.PromotionCode
                },
                "booking.events",
                 "topic",
                 "booking.cancelled",
                 cancellationToken
            );
            throw; 
        }

        return booking.Id;
    }
}
