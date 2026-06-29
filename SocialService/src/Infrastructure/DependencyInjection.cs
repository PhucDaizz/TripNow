using SocialService.Infrastructure.Protos;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nexus.BuildingBlocks.Extensions;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;
using SocialService.Domain.Repositories;
using SocialService.Infrastructure.BackgroundJobs.Consumer;
using SocialService.Infrastructure.Data.Repositories;
using SocialService.Infrastructure.Services;
using SocialService.Infrastructure.Settings;

namespace SocialService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<ServiceUrlOptions>(
                configuration.GetSection(ServiceUrlOptions.SectionName));
            services.Configure<CloudinarySettings>(
               configuration.GetSection(CloudinarySettings.SectionName)
            );

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseMySql(
                   configuration.GetConnectionString("DefaultConnection"),
                   new MySqlServerVersion(new Version(8, 0, 21)),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IPostLikeRepository, PostLikeRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ISavedPostRepository, SavedPostRepository>();
            services.AddScoped<IUserFollowRepository, UserFollowRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IImageProcessor, ImageSharpProcessor>();
            services.AddSingleton<ICloudinaryService, CloudinaryService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IAuthorIdentityService, AuthorIdentityService>();
            services.AddScoped<IIntegrationEventService, IntegrationEventService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IHotelCatalogService, HotelCatalogService>();

            services.AddHostedService<MemberEventsConsumer>();
            services.AddHostedService<HotelEventsConsumer>();


            services.AddGrpcClient<AuthGrpc.AuthGrpcClient>((sp, options) =>
            {
                var serviceUrls = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                options.Address = new Uri(serviceUrls.Auth);
            });
            services.AddGrpcClient<CatalogGrpc.CatalogGrpcClient>((sp, options) =>
            {
                var serviceUrls = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                options.Address = new Uri(serviceUrls.HotelCatalog);
            });
            services.AddGrpcClient<BookingGrpc.BookingGrpcClient>((sp, options) =>
            {
                var serviceUrls = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                options.Address = new Uri(serviceUrls.Booking);
            });

            services.AddSharedRabbitMQ(configuration);

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
