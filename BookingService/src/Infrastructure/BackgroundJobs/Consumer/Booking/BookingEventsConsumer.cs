using BookingService.Application.DTOs.Booking.Event;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;

namespace BookingService.Infrastructure.BackgroundJobs.Consumer.Booking
{
    public class BookingEventsConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly ILogger<BookingEventsConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public BookingEventsConsumer(IMessageConsumer consumer, ILogger<BookingEventsConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _consumer = consumer;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Cập nhật booking thanh toán thành công
            await _consumer.Subscribe<PaymentSucceededEvent>(
                exchange: "payment.events",
                exchangeType: "topic",
                routingKey: "payment.success",
                queueName: "booking-service-success-book",
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
