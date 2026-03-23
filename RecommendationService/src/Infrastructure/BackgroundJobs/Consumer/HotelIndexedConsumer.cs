using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;
using RecommendationService.Application.Features.Hotel.EventHandlers.HotelIndexedIntegration;

namespace RecommendationService.Infrastructure.BackgroundJobs.Consumer
{
    public class HotelIndexedConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<HotelIndexedConsumer> _logger;

        public HotelIndexedConsumer(
            IMessageConsumer consumer,
            IServiceScopeFactory scopeFactory,
            ILogger<HotelIndexedConsumer> logger)
        {
            _consumer     = consumer;
            _scopeFactory = scopeFactory;
            _logger       = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subscribe<HotelIndexedIntegrationEvent>(
                exchange    : "hotel-catalog.events",
                exchangeType: "topic",
                routingKey  : "hotel.published",
                queueName   : "recommendation-service-hotel-published",
                handler     : (msg) => ProcessMessage(msg, stoppingToken));
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
