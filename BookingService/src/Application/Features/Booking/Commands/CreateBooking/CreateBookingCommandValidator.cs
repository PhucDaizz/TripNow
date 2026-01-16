using FluentValidation;

namespace BookingService.Application.Features.Booking.Commands.CreateBooking
{
    public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingCommandValidator()
        {
            RuleFor(x => x.HotelId)
                .NotEmpty().WithMessage("Hotel ID is required.");

            RuleFor(x => x.CheckInDate)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Check-in date cannot be in the past.");

            RuleFor(x => x.CheckOutDate)
                .GreaterThan(x => x.CheckInDate)
                .WithMessage("Check-out date must be after check-in date.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one room must be selected.")
                .Must(items => items != null && items.Count > 0)
                .WithMessage("Booking items list cannot be empty.");

            RuleForEach(x => x.Items).SetValidator(new BookingItemDtoValidator());

            RuleFor(x => x.Source)
                .IsInEnum().WithMessage("Invalid booking source.");

            When(x => !string.IsNullOrEmpty(x.PromotionCode), () =>
            {
                RuleFor(x => x.PromotionId)
                    .NotNull()
                    .NotEqual(Guid.Empty)
                    .WithMessage("Promotion ID is required when a Promotion Code is provided.");
            });

            When(x => x.PromotionId.HasValue, () =>
            {
                RuleFor(x => x.PromotionCode)
                    .NotEmpty()
                    .WithMessage("Promotion Code is required when a Promotion ID is provided.");
            });
        }
    }

    public class BookingItemDtoValidator : AbstractValidator<BookingItemDto>
    {
        public BookingItemDtoValidator()
        {
            RuleFor(x => x.RoomTypeId)
                .NotEmpty().WithMessage("Room Type ID is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");
        }
    }
}
