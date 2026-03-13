using MediatR;
namespace SocialService.Application.Features.Member.EventHandlers.HotelCreated
{
    public class HotelCreatedEvent: INotification
    {
        public Guid HotelId { get; init; }
        public required string Name { get; init; }
    }
}
