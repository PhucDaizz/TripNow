using HotelCatalogService.Application.Features.Hotel.EventHandlers.IncreaseFollowHotel;
using HotelCatalogService.Application.Features.Hotel.EventHandlers.UnfollowHotel;
using HotelCatalogService.Application.Features.Room.EventHandlers.BookingExpired;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;

namespace HotelCatalogService.Infrastructure.BackgroundJobs.Consumer.Social
{
    public class SocialEventsConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SocialEventsConsumer> _logger;

        public SocialEventsConsumer(IMessageConsumer consumer,
            IServiceScopeFactory scopeFactory,
            ILogger<SocialEventsConsumer> logger)
        {
            _consumer = consumer;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subscribe<IncreaseFollowHotelEvent>(
                exchange: "social.events",
                exchangeType: "topic",
                routingKey: "increase.follow.hotel",
                queueName: "hotel-service-increase-follow",
                handler: (msg) => ProcessMessage(msg, stoppingToken));
           
            await _consumer.Subscribe<UnfollowHotelEvent>(
                exchange: "social.events",
                exchangeType: "topic",
                routingKey: "unfollow.hotel",
                queueName: "hotel-service-unfollow",
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
