using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.Contracts;
using HotelCatalogService.Domain.Repositories;
using HotelCatalogService.Infrastructure.BackgroundJobs.Consumer.Booking;
using HotelCatalogService.Infrastructure.BackgroundJobs.Consumer.Room;
using HotelCatalogService.Infrastructure.BackgroundJobs.Consumer.Social;
using HotelCatalogService.Infrastructure.Data.Repositories;
using HotelCatalogService.Infrastructure.Protos;
using HotelCatalogService.Infrastructure.Services;
using HotelCatalogService.Infrastructure.Settings;
using HotelCatalogService.Infrastructure.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nexus.BuildingBlocks.Extensions;

namespace HotelCatalogService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));
            services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
            services.Configure<ServiceUrlOptions>(configuration.GetSection(ServiceUrlOptions.SectionName));

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseMySql(
                   configuration.GetConnectionString("DefaultConnection"),
                   new MySqlServerVersion(new Version(8, 0, 21)),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IAmenityRepository, AmenityRepository>();
            services.AddScoped<ICancellationPolicyRepository, CancellationPolicyRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IEmailServices, EmailServices>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IHousekeepingSignalRService, HousekeepingSignalRService>();
            services.AddSingleton<ICloudinaryService, CloudinaryService>();
            services.AddSingleton<IImageProcessor, ImageSharpProcessor>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IStaffService, StaffService>();

            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IIntegrationEventService, IntegrationEventService>();

            services.AddSharedRabbitMQ(configuration);
            services.AddHostedService<BookingCancelledConsumer>();
            services.AddHostedService<BookingEventsConsumer>();
            services.AddHostedService<SocialEventsConsumer>();

            services.AddGrpcClient<StaffProfileGrpc.StaffProfileGrpcClient>((sp, options) =>
            {
                var serviceUrls = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                options.Address = new Uri(serviceUrls.Auth);
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
            services.AddGrpcClient<BookingGrpc.BookingGrpcClient>((sp, options) =>
            {
                var serviceUrls = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                options.Address = new Uri(serviceUrls.Booking);
            });

            services.AddSignalR();

            return services;
        }

        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
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
