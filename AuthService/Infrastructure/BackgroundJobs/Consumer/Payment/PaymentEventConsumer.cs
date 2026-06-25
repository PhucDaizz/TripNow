using Application.DTOs.Payment;
using Infrastructure.BackgroundJobs.Consumer.Hotel;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;

namespace Infrastructure.BackgroundJobs.Consumer.Payment
{
    public class PaymentEventConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<HotelEventConsumer> _logger;

        public PaymentEventConsumer(IMessageConsumer messageConsumer, IServiceScopeFactory scopeFactory, ILogger<HotelEventConsumer> logger)
        {
            _consumer = messageConsumer;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subscribe<RefundRequestCompleted>(
                exchange: "payment.events",
                exchangeType: "topic",
                routingKey: "refundrequest.completed",
                queueName: "auth-service-refundrequest-completed",
                handler: (msg) => ProcessMessage(msg, stoppingToken)
            );
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
