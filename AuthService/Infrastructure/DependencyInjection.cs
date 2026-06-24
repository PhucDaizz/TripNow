using Application.Common.Interfaces;
using Application.Contracts;
using Application.Repositories;
using Domain.Repositories;
using Infrastructure.BackgroundJobs.Consumer.Hotel;
using Infrastructure.BackgroundJobs.Consumer.Payment;
using Infrastructure.BackgroundJobs.Consumer.User;
using Infrastructure.Contracts;
using Infrastructure.Data.Repositories;
using Infrastructure.Persistence.SeedData;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nexus.BuildingBlocks.Extensions;


namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Cấu hình Settings
            services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));
            services.Configure<AdminAccountOptions>(configuration.GetSection(AdminAccountOptions.SectionName));
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
            services.Configure<GoogleSettings>(configuration.GetSection(GoogleSettings.SectionName));
            services.Configure<FrontendSettings>(configuration.GetSection(FrontendSettings.SectionName));

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                                           Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto |
                                           Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost;

                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            // 2. Cấu hình Database
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseMySql(
                   configuration.GetConnectionString("DefaultConnection"),
                   new MySqlServerVersion(new Version(8, 0, 21)),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
            
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            //3. Các Service nghiệp vụ nội bộ
            services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IStaffProfileRepository, StaffProfileRepository>();

            services.AddScoped<IEmailServices, EmailServices>();
            services.AddScoped<IExternalAuthService, ExternalAuthService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton<ICloudinaryService, CloudinaryService>();
            services.AddSingleton<IImageProcessor, ImageSharpProcessor>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IIntegrationEventService, IntegrationEventService>();
            services.AddScoped<IExternalAuthService, ExternalAuthService>();

            // 4. Đăng ký Message Broker & Background Jobs
            services.AddSharedRabbitMQ(configuration);
            services.AddHostedService<UserEventsConsumer>();
            services.AddHostedService<HotelEventConsumer>();
            services.AddHostedService<PaymentEventConsumer>();

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

                var initializer = services.GetRequiredService<IDbInitializer>();
                await initializer.SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>(); 
                logger.LogError(ex, "Lỗi khi chạy Migration hoặc tải dữ liệu mẫu (Seed Data).");
            }
        }
    }
}
