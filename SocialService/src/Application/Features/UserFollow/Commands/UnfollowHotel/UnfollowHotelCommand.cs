using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.UserFollow.Commands.UnfollowHotel
{
    public class UnfollowHotelCommand: IRequest<Result<bool>>
    {
        public Guid HotelId { get; set; }
    }
}
