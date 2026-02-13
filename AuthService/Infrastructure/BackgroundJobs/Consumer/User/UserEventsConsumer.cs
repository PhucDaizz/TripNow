using Application.DTOs.User;
using Application.DTOs.User.Event;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;

namespace Infrastructure.BackgroundJobs.Consumer.User
{
    public class UserEventsConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<UserEventsConsumer> _logger;

        public UserEventsConsumer(
            IMessageConsumer consumer,
            IServiceScopeFactory scopeFactory,
            ILogger<UserEventsConsumer> logger
            )
        {
            _consumer = consumer;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subscribe<SendEmailConfirmation>(
                exchange: "user.events",
                exchangeType: "topic",
                routingKey: "user.registered",
                queueName: "auth-service-user-registered",
                handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<IncreaseFollowEvent> (
                exchange: "social.events",
                exchangeType: "topic",
                routingKey: "increase.follow.user",
                queueName: "auth-service-increase-follow",
                handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<UnfollowEvent>(
                exchange: "social.events",
                exchangeType: "topic",
                routingKey: "unfollow.user",
                queueName: "auth-service-unfollow",
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
