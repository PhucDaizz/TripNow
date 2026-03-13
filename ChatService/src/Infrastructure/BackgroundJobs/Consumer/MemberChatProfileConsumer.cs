using ChatService.Application.Features.ChatProfile.EventHandlers.MemberChangeImage;
using ChatService.Application.Features.ChatProfile.EventHandlers.MemberRegister;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;

namespace ChatService.Infrastructure.BackgroundJobs.Consumer
{
    public class MemberChatProfileConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly ILogger<MemberChatProfileConsumer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public MemberChatProfileConsumer(IMessageConsumer consumer, ILogger<MemberChatProfileConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _consumer = consumer;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subscribe<MemberRegisterEvent>(
               exchange: "user.events",
               exchangeType: "topic",
               routingKey: "user.registered",
               queueName: "chat-service-member-registered",
               handler: (msg) => ProcessMessage(msg, stoppingToken));

            await _consumer.Subscribe<MemberChangeImageEvent>(
               exchange: "user.events",
               exchangeType: "topic",
               routingKey: "user.avatar_changed",
               queueName: "chat-service.member.avatar_changed",
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
