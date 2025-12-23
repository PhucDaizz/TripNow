using Application.DTOs.Hotel;
using Application.DTOs.User;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Interfaces;

namespace Infrastructure.BackgroundJobs.Consumer.Hotel
{
    public class HotelEventConsumer : BackgroundService
    {
        private readonly IMessageConsumer _consumer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<HotelEventConsumer> _logger;

        public HotelEventConsumer(IMessageConsumer messageConsumer, IServiceScopeFactory scopeFactory, ILogger<HotelEventConsumer> logger)
        {
            _consumer = messageConsumer;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.Subscribe<HotelApproved>(
                exchange: "hotel.events",
                exchangeType: "topic",
                routingKey: "hotel.approved",
                queueName: "hotel-service-approved",
                handler: async (eventMessage) =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    try
                    {
                        await mediator.Publish(eventMessage, stoppingToken);
                        _logger.LogInformation("Successfully processed SendEmailHotelApproved for user: {UserId}", eventMessage.OwnerId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error handling message for user: {UserId}",
                            eventMessage.OwnerId);
                    }
                });
        }
    }
}
