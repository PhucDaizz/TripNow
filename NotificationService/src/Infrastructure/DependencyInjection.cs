using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nexus.BuildingBlocks.Extensions;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Services;
using NotificationService.Domain.Repositories;
using NotificationService.Infrastructure.BackgroundJobs.Consumer;
using NotificationService.Infrastructure.Data.Repositories;
using NotificationService.Infrastructure.Protos;
using NotificationService.Infrastructure.Services;
using NotificationService.Infrastructure.Settings;

namespace NotificationService.Infrastructure
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

            services.AddTransient<INotificationService, SignalR.NotificationService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ISocialNotificationRepository, SocialNotificationRepository>();

            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IIntegrationEventService, IntegrationEventService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IHotelAuthorizationService, HotelAuthorizationService>();

            services.AddHostedService<SocialNotificationConsumer>();
            services.AddHostedService<SystemNotificationConsumer>();

            services.AddGrpcClient<CatalogGrpc.CatalogGrpcClient>((sp, options) =>
            {
                var serviceUrls = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                options.Address = new Uri(serviceUrls.HotelCatalog);
            });

            services.AddSharedRabbitMQ(configuration);
            return services;
        }
    }
}
