using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelCatalogService.Application.Features.Hotel.Commands.UpdateHotel
{
    public class UpdateHotelCommandValidator : AbstractValidator<UpdateHotelCommand>
    {
        public UpdateHotelCommandValidator()
        {
            RuleFor(x => x.HotelId)
                .NotEmpty().WithMessage("HotelId là bắt buộc.");

            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("OwnerId là bắt buộc.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên khách sạn không được để trống.")
                .MaximumLength(250).WithMessage("Tên khách sạn không được vượt quá 250 ký tự.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Mô tả không được để trống.")
                .MinimumLength(20).WithMessage("Mô tả phải có ít nhất 20 ký tự.");

            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Địa chỉ đường không được để trống.")
                .MaximumLength(200).WithMessage("Địa chỉ đường quá dài.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Thành phố không được để trống.")
                .MaximumLength(100).WithMessage("Tên thành phố quá dài.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Quốc gia không được để trống.")
                .MaximumLength(100).WithMessage("Tên quốc gia quá dài.");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Vĩ độ (Latitude) phải nằm trong khoảng từ -90 đến 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Kinh độ (Longitude) phải nằm trong khoảng từ -180 đến 180.");
        }
    }
}
