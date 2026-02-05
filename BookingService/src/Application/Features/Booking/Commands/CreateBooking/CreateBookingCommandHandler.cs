using BookingService.Application.Common.Interfaces;
using BookingService.Application.Contracts;
using BookingService.Application.DTOs.Booking;
using BookingService.Application.DTOs.HotelCatalog;
using BookingService.Domain.Exceptions;
using BookingService.Domain.ValueObject;
using Domain.Common.Response;
using MediatR;

namespace BookingService.Application.Features.Booking.Commands.CreateBooking;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Result<CreateBookingResponse>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHotelCatalogService _hotelCatalogService;
    private readonly IIntegrationEventService _integrationEventService;
    private readonly IPaymentService _paymentService;

    public CreateBookingCommandHandler(ICurrentUserService currentUserService, 
        IUnitOfWork unitOfWork, 
        IHotelCatalogService hotelCatalogService,
        IIntegrationEventService integrationEventService, 
        IPaymentService paymentService)
    {
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _hotelCatalogService = hotelCatalogService;
        _integrationEventService = integrationEventService;
        _paymentService = paymentService;
    }

    public async Task<Result<CreateBookingResponse>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId == null || !Guid.TryParse(_currentUserService.UserId, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated or invalid user ID.");
        }

        var hotel = await _hotelCatalogService.GetHotelSummary(request.HotelId, cancellationToken);
        if (hotel == null)
        {
            throw new DomainException("The specified hotel does not exist.");
        }

        var listRoomTypeIds = request.Items.Select(i => i.RoomTypeId).Distinct().ToList();

        var inventoryList = await _unitOfWork.Inventory.GetInventoriesInRangeAsync(
            listRoomTypeIds,
            request.CheckInDate,
            request.CheckOutDate,
            cancellationToken
        );

        var inventoryLookup = inventoryList
            .ToDictionary(k => new { k.RoomTypeId, k.Date }, v => v);

        int nights = request.CheckOutDate.DayNumber - request.CheckInDate.DayNumber;
        if (inventoryList.Count < nights)
        {
            throw new DomainException("There is no room schedule for this time period yet..");
        }

        foreach (var item in request.Items)
        {
            for (var date = request.CheckInDate; date < request.CheckOutDate; date = date.AddDays(1))
            {
                var key = new { item.RoomTypeId, Date = date };

                if (!inventoryLookup.TryGetValue(key, out var inventory))
                {
                    throw new DomainException($"Room type {item.RoomTypeId} has not been opened for sale on {date:dd/MM/yyyy}.");
                }

                if (!inventory.TryReserve(item.Quantity))
                {
                    throw new DomainException($"The room type {item.RoomTypeId} is fully booked on {date:dd/MM/yyyy}.");
                }
            }
        }


        var booking = new Domain.Entities.Booking(
            userId,
            _currentUserService.FullName,
            _currentUserService.Email,
            request.HotelId,
            hotel.HotelName,
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

        var roomDataLookup = allRoomPrices.ToDictionary(k => k.RoomTypeId, v => v);

        decimal tempBaseTotal = 0;
        int totalNights = request.CheckOutDate.DayNumber - request.CheckInDate.DayNumber;

        if (totalNights <= 0) throw new DomainException("The check-out date must be after the check-in date.");

        foreach (var item in request.Items)
        {
            if (!roomDataLookup.TryGetValue(item.RoomTypeId, out var roomPriceData))
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

            var bookingItem = booking.AddItem(item.RoomTypeId, item.Quantity, avgUnitPrice);

            if (roomPriceData.CancellationPolicy != null)
            {
                var snapshotRules = roomPriceData.CancellationPolicy.Rules.Select(r => new PolicyRuleSnapshot
                {
                    HoursBeforeCheckIn = r.HoursBeforeCheckIn,
                    RefundPercentage = r.RefundPercentage
                }).ToList();

                bookingItem.SetPolicySnapshot(roomPriceData.CancellationPolicy.Name, snapshotRules);
            }

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

        var paymentResult = await _paymentService.CreatePaymentLinkAsync(booking.Id, booking.TotalAmount, userId, request.paymentProvider, cancellationToken);

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

        return Result.Success(new CreateBookingResponse
        {
            BookingId = booking.Id,
            PaymentUrl = paymentResult
        });
    }
}
