using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;
using PaymentService.Application.DTOs.OwnerWallet.Event;
using PaymentService.Application.DTOs.Payment.Event;

namespace PaymentService.Infrastructure.BackgroundJobs.Consumer.OwnerWallet
{
    public class OwnerWalletEventsConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly ILogger<OwnerWalletEventsConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        public OwnerWalletEventsConsumer(IMessageConsumer consumer, ILogger<OwnerWalletEventsConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _consumer = consumer;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subscribe<RegisterHotelOwer>(
               exchange: "user.events",
               exchangeType: "topic",
               routingKey: "user.hotelowner.registered",
               queueName: "payment-service-hotelowner-registered",
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
