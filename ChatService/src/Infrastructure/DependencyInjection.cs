using ChatService.Application.Common.Interfaces;
using ChatService.Application.Interface;
using ChatService.Domain.Repositories;
using ChatService.Infrastructure.BackgroundJobs.Consumer;
using ChatService.Infrastructure.Data.Repositories;
using ChatService.Infrastructure.Protos;
using ChatService.Infrastructure.Services;
using ChatService.Infrastructure.Settings;
using ChatService.Infrastructure.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nexus.BuildingBlocks.Extensions;

namespace ChatService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ServiceUrlOptions>(
               configuration.GetSection(ServiceUrlOptions.SectionName));

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseMySql(
                   configuration.GetConnectionString("DefaultConnection"),
                   new MySqlServerVersion(new Version(8, 0, 21)),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddSignalR();

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IChatProfileRepository, ChatProfileRepository>();

            services.AddTransient<IChatNotificationService, ChatNotificationService>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IIntegrationEventService, IntegrationEventService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IHotelAuthorizationService, HotelAuthorizationService>();
            services.AddScoped<IRecommendationService, RecommendationService>();
            services.AddScoped<IHotelCatalogService, HotelCatalogService>();

            services.AddHostedService<HotelChatProfileConsumer>();
            services.AddHostedService<MemberChatProfileConsumer>();


            services.AddGrpcClient<CatalogGrpc.CatalogGrpcClient>((sp, options) =>
            {
                var serviceUrls = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                options.Address = new Uri(serviceUrls.HotelCatalog);
            });
            services.AddGrpcClient<RagGrpc.RagGrpcClient>((sp, options) =>
            {
                var serviceUrls = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                options.Address = new Uri(serviceUrls.Recommendation);
            });

            services.AddHttpClient<IAiChatService, OpenRouterChatService>();

            services.AddSharedRabbitMQ(configuration);

            return services;
        }
    }
}
