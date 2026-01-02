using HotelCatalogService.Application.Features.Promotion.EventHandlers.BookingCancelled;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;

namespace HotelCatalogService.Infrastructure.BackgroundJobs.Consumer.Booking
{
    public class BookingCancelledConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BookingCancelledConsumer> _logger;

        public BookingCancelledConsumer(IMessageConsumer consumer,
            IServiceScopeFactory scopeFactory,
            ILogger<BookingCancelledConsumer> logger)
        {
            _consumer = consumer;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subscribe<BookingCancelledEvent>(
                exchange: "booking.events",              
                exchangeType: "topic",
                routingKey: "booking.cancelled",         
                queueName: "hotel-service-booking-cancelled", 
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
