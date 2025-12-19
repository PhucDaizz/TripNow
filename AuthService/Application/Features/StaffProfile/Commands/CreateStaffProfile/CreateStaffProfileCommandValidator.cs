using FluentValidation;

namespace Application.Features.StaffProfile.Commands.CreateStaffProfile
{
    public class CreateStaffProfileCommandValidator : AbstractValidator<CreateStaffProfileCommand>
    {
        private readonly List<string> _validPositions = new()
        {
            "Receptionist", "Housekeeping", "HotelOwner", "SysAdmin"
        };

        public CreateStaffProfileCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .MaximumLength(450).WithMessage("Email is too long")
                .MinimumLength(12).WithMessage("Too short");

            RuleFor(x => x.HotelId)
                .NotEmpty().WithMessage("Hotel ID is required");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("Position is required")
                .Must(IsValidPosition).WithMessage($"Position must be one of: {string.Join(", ", _validPositions)}");

            RuleFor(x => x)
                .MustAsync(async (command, cancellation) =>
                {
                    // Check if user exists (async validation)
                    // This would be implemented in a custom validator
                    return true;
                }).WithMessage("User does not exist");
        }

        private bool IsValidPosition(string position)
        {
            return _validPositions.Contains(position, StringComparer.OrdinalIgnoreCase);
        }
    }
}
