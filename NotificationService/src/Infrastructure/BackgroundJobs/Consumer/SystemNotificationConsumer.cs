using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;
using NotificationService.Application.DTOs.SocialNotification.Event;
using NotificationService.Application.DTOs.SystemNotification.Event;

namespace NotificationService.Infrastructure.BackgroundJobs.Consumer
{
    public class SystemNotificationConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly ILogger<SystemNotificationConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public SystemNotificationConsumer(IMessageConsumer consumer, ILogger<SystemNotificationConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _consumer = consumer;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subscribe<SystemNotificationCreateEvent>(
               exchange: "social.events",
               exchangeType: "topic",
               routingKey: "new.notification.system",
               queueName: "notification-service-create-system",
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
