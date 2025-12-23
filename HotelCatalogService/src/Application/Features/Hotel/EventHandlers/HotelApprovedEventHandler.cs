using HotelCatalogService.Domain.Events.Hotel;
using MediatR;
using Nexus.BuildingBlocks.Interfaces;
using RabbitMQ.Client;

namespace HotelCatalogService.Application.Features.Hotel.EventHandlers
{
    public class HotelApprovedEventHandler : INotificationHandler<HotelApprovedEvent>
    {
        private readonly IMessagePublisher _publisher;

        public HotelApprovedEventHandler(IMessagePublisher publisher )
        {
            _publisher = publisher;
        }

        public async Task Handle(HotelApprovedEvent notification, CancellationToken cancellationToken)
        {
            await _publisher.PublishAsync("hotel.events", ExchangeType.Topic, "hotel.approved", notification);
        }
    }
}
