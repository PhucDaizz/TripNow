using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;
using NotificationService.Application.DTOs.SocialNotification.Event;
using NotificationService.Application.Features.SocialNotification.EventHandlers;

namespace NotificationService.Infrastructure.BackgroundJobs.Consumer
{
    public class SocialNotificationConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly ILogger<SocialNotificationConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public SocialNotificationConsumer(IMessageConsumer consumer, ILogger<SocialNotificationConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _consumer = consumer;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subscribe<CommentCreateEvent>(
               exchange: "social.events",
               exchangeType: "topic",
               routingKey: "new.comment.create",
               queueName: "notification-service-new-comment",
               handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<PostLikedIntegrationEvent>(
               exchange: "social.events",
               exchangeType: "topic",
               routingKey: "new.post.like",
               queueName: "notification-service-post-like",
               handler: (msg) => ProcessMessage(msg, stoppingToken));
            
            await _consumer.Subscribe<PostUnlikedIntegrationEvent>(
               exchange: "social.events",
               exchangeType: "topic",
               routingKey: "post.unlike",
               queueName: "notification-service-post-unlike",
               handler: (msg) => ProcessMessage(msg, stoppingToken));
            
            await _consumer.Subscribe<UserFollowedIntegrationEvent>(
               exchange: "social.events",
               exchangeType: "topic",
               routingKey: "new.user.follow",
               queueName: "notification-service-new-follow-user",
               handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<UserUnfollowedIntegrationEvent>(
               exchange: "social.events",
               exchangeType: "topic",
               routingKey: "remove.user.follow",
               queueName: "notification-service-unfollow-user",
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
