using BookingService.Application.DTOs.Inventory;
using BookingService.Application.DTOs.InventoryConfiguration;
using BookingService.Application.Features.InventoryConfiguration.EventHandlers.RoomTypeCreated;
using BookingService.Application.Features.InventoryConfiguration.EventHandlers.RoomTypeDeletedEvent;
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

            await _consumer.Subscribe<RoomMovedToAnotherRoomTypeEvent>( // đã làm conf
                exchange: "hotel-catalog.events",
                exchangeType: "topic",
                routingKey: "room.update",
                queueName: "booking-service-update-room",
                handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<InventoryStockChangedEvent>( // đã làm conf
                exchange: "hotel-catalog.events",
                exchangeType: "topic",
                routingKey: "room.range.created",
                queueName: "booking-service-inventory-change", 
                handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<RoomMaintenanceScheduledEvent>(  // lịch trong tháng cố định đã đoán trước không cần conf
                exchange: "hotel-catalog.events",
                exchangeType: "topic",
                routingKey: "room.maintain",
                queueName: "booking-service-maintain-room", 
                handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<RoomMaintenanceFinishedEvent>( // lịch trong tháng cố định đã đoán trước không cần conf
                exchange: "hotel-catalog.events",
                exchangeType: "topic",
                routingKey: "room.finished.maintain",
                queueName: "booking-service-finished-maintain-room", 
                handler: (msg) => ProcessMessage(msg, stoppingToken));
            
            await _consumer.Subscribe<RoomTypeCreatedEvent>( // đã làm conf
                exchange: "hotel-catalog.events",
                exchangeType: "topic",
                routingKey: "roomtype.create",
                queueName: "booking-service-room-type-create", 
                handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<RoomTypeDeletedEvent>( 
                exchange: "hotel-catalog.events",
                exchangeType: "topic",
                routingKey: "roomtype.delete",
                queueName: "booking-service-room-type-delete", 
                handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<HotelStatusChangedEvent>(  // vui lòng gọi hàm mở khách sạn để inventory hoạt động lại
                exchange: "hotel-catalog.events",
                exchangeType: "topic",
                routingKey: "hotel.close.temporary",
                queueName: "booking-service-hotel-close-temporary", 
                handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<HotelStatusChangedEvent>(
                exchange: "hotel-catalog.events",
                exchangeType: "topic",
                routingKey: "hotel.reopen",
                queueName: "booking-service-hotel-reopen", 
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
