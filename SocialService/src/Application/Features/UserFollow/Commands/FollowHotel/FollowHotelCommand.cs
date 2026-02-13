using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.UserFollow.Commands.FollowHotel
{
    public class FollowHotelCommand : IRequest<Result<bool>>
    {
        public Guid HotelId { get; set; }
    }
}
