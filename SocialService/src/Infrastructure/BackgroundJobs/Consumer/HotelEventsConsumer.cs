using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;
using SocialService.Application.Features.Member.EventHandlers.HotelCreated;

namespace SocialService.Infrastructure.BackgroundJobs.Consumer
{
    public class HotelEventsConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly ILogger<HotelEventsConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public HotelEventsConsumer(IMessageConsumer consumer, ILogger<HotelEventsConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _consumer = consumer;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subscribe<HotelCreatedEvent>(
               exchange: "hotel-catalog.events",
               exchangeType: "topic",
               routingKey: "hotel.created",
               queueName: "social-service-hotel-created", 
               handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<HotelThumbnailChangedIntegrationEvent>(
               exchange: "hotel-catalog.events",
               exchangeType: "topic",
               routingKey: "hotel.thumbnail_changed",
               queueName: "social-service-hotel-thumbnail-changed", 
               handler: (msg) => ProcessMessage(msg, stoppingToken));

        }

        private async Task ProcessMessage<TMessage>(TMessage message, CancellationToken token) where TMessage : class
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            try
            {
                await mediator.Publish(message, token);

                _logger.LogInformation("Successfully processed message {MessageType} : {MessageData}",
                    typeof(TMessage).Name, System.Text.Json.JsonSerializer.Serialize(message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling message {MessageType}", typeof(TMessage).Name);
            }
        }
    }
}
