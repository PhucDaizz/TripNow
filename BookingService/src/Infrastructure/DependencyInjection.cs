using BookingService.Application.Common.Interfaces;
using BookingService.Domain.Repositories;
using BookingService.Infrastructure.BackgroundJobs;
using BookingService.Infrastructure.BackgroundJobs.Consumer.Booking;
using BookingService.Infrastructure.BackgroundJobs.Consumer.Inventory;
using BookingService.Infrastructure.Data.Repositories;
using BookingService.Infrastructure.Protos;
using BookingService.Infrastructure.Services;
using BookingService.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nexus.BuildingBlocks.Extensions;

namespace BookingService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<InventorySettings>(
               configuration.GetSection(InventorySettings.SectionName)
            );

            services.Configure<ServiceUrlOptions>(
                configuration.GetSection(ServiceUrlOptions.SectionName));

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseMySql(
                   configuration.GetConnectionString("DefaultConnection"),
                   new MySqlServerVersion(new Version(8, 0, 21)),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IInventorySettings>(sp =>
                sp.GetRequiredService<IOptions<InventorySettings>>().Value);


            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingPriceSnapshotRepository, BookingPriceSnapshotRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IInventoryConfigurationRepository, InventoryConfigurationRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IIntegrationEventService, IntegrationEventService>();
            services.AddScoped<IHotelAuthorizationService, HotelAuthorizationService>();

            services.AddSharedRabbitMQ(configuration);
            services.AddHostedService<InventoryEventsConsumer>();
            services.AddHostedService<BookingEventsConsumer>();
            services.AddHostedService<BookingCleanupWorker>();
            services.AddHostedService<DailyInventoryRolloutJob>();

            services.AddGrpcClient<CatalogGrpc.CatalogGrpcClient>((sp, options) =>
            {
                var serviceUrlOptions = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                options.Address = new Uri(serviceUrlOptions.HotelCatalog);
            })
            .AddCallCredentials((context, metadata, serviceProvider) =>
            {
                var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                var authHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

                if (!string.IsNullOrEmpty(authHeader))
                {
                    metadata.Add("Authorization", authHeader);
                }

                return Task.CompletedTask;
            });

            services.AddGrpcClient<PaymentGrpc.PaymentGrpcClient>((sp, options) =>
            {
                var serviceUrlOptions = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                options.Address = new Uri(serviceUrlOptions.Payment);
            });

            return services;
        }

        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                if (context.Database.GetPendingMigrations().Any())
                {
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }
    }
}
