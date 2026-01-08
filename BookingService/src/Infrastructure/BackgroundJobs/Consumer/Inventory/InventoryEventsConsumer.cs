using BookingService.Application.DTOs.Inventory;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;

namespace BookingService.Infrastructure.BackgroundJobs.Consumer.Inventory
{
    public class InventoryEventsConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly ILogger<InventoryEventsConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public InventoryEventsConsumer(IMessageConsumer consumer, ILogger<InventoryEventsConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _consumer = consumer;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subscribe<AddRoomInventoryEvent>(
                exchange: "hotel-catalog.events",
                exchangeType: "topic",
                routingKey: "room.created",
                queueName: "booking-service-add-room",
                handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<DeductRoomInventoryEvent>(
                exchange: "hotel-catalog.events",
                exchangeType: "topic",
                routingKey: "room.deduct",
                queueName: "booking-service-deduct-room",
                handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<RoomMovedToAnotherRoomTypeEvent>(
                exchange: "hotel-catalog.events",
                exchangeType: "topic",
                routingKey: "room.update",
                queueName: "booking-service-update-room",
                handler: (msg) => ProcessMessage(msg, stoppingToken));
            await _consumer.Subscribe<InventoryStockChangedEvent>(
                exchange: "hotel-catalog.events",
                exchangeType: "topic",
                routingKey: "room.range.created",
                queueName: "booking-service-inventory-change", 
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
