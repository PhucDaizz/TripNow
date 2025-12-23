using FluentValidation;

namespace HotelCatalogService.Application.Features.Hotel.Commands.CreateHotel
{
    public class CreateHotelCommandValidator : AbstractValidator<CreateHotelCommand>
    {
        public CreateHotelCommandValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Tên khách sạn không được để trống.")
                .MaximumLength(250).WithMessage("Tên khách sạn không quá 250 ký tự.");

            RuleFor(v => v.OwnerId)
                .NotEmpty().WithMessage("Chủ sở hữu không hợp lệ.");

            RuleFor(v => v.City)
                .NotEmpty().WithMessage("Thành phố không được để trống.");

            RuleFor(v => v.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Vĩ độ không hợp lệ.");

            RuleFor(v => v.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Kinh độ không hợp lệ.");
        }
    }
}
