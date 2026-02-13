using MediatR;

namespace SocialService.Application.Features.UserFollow.Commands.UnfollowHotel
{
    public class UnfollowHotelCommand: INotification
    {
        public Guid HotelId { get; set; }
    }
}
