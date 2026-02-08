using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;
using PaymentService.Application.DTOs.Payment.Event;

namespace PaymentService.Infrastructure.BackgroundJobs.Consumer.Payment
{
    public class PaymentEventsConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly ILogger<PaymentEventsConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        public PaymentEventsConsumer(IMessageConsumer consumer, ILogger<PaymentEventsConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _consumer = consumer;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subscribe<BookingCancelledWithOutPay>(
               exchange: "booking.events",
               exchangeType: "topic",
               routingKey: "booking.cancelled.notpay",
               queueName: "payment-service-cancelled-withoutpay",
               handler: (msg) => ProcessMessage(msg, stoppingToken));
            await _consumer.Subscribe<BookingCompleted>(
               exchange: "booking.events",
               exchangeType: "topic",
               routingKey: "booking.completed",
               queueName: "payment-service-checkout-success",
               handler: (msg) => ProcessMessage(msg, stoppingToken));
            await _consumer.Subscribe<BookingRefund>(
               exchange: "booking.events",
               exchangeType: "topic",
               routingKey: "booking.refund",
               queueName: "payment-service-refund",
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
