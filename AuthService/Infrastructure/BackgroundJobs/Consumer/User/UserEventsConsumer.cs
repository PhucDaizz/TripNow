using Application.DTOs.User;
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
                handler: async (eventMessage) =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    try
                    {
                        await mediator.Publish(eventMessage, stoppingToken);
                        _logger.LogInformation("Successfully processed SendEmailConfirmation for user: {UserId}", eventMessage.UserId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error handling message for user: {UserId}",
                            eventMessage.UserId);
                    }
                });
        }
    }
}
