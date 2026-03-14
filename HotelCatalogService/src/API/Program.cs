using DotNetEnv;
using HotelCatalogService.API.Common.ExceptionHandling;
using HotelCatalogService.API.ExceptionHandling;
using HotelCatalogService.API.Extensions;
using HotelCatalogService.API.StartUp;
using HotelCatalogService.Application;
using HotelCatalogService.Application.Contracts;
using HotelCatalogService.Infrastructure;
using HotelCatalogService.Infrastructure.BackgroundJobs.Consumer.Booking;
using HotelCatalogService.Infrastructure.BackgroundJobs.Consumer.Room;
using HotelCatalogService.Infrastructure.BackgroundJobs.Consumer.Social;
using HotelCatalogService.Infrastructure.Hubs;
using HotelCatalogService.Infrastructure.Services;
using HotelCatalogService.Infrastructure.Settings;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nexus.BuildingBlocks.Extensions;
using System.Diagnostics;

namespace HotelCatalogService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var envPath = Path.Combine(Directory.GetCurrentDirectory(), "../Config/.env");
            if (File.Exists(envPath))
            {
                Env.Load(envPath);
            }

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<CloudinarySettings>(
               builder.Configuration.GetSection(CloudinarySettings.SectionName)
            );
            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection(EmailSettings.SectionName)
            );

            builder.Services.Configure<ServiceUrlOptions>(
                builder.Configuration.GetSection(ServiceUrlOptions.SectionName));


            builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddHealthChecks();

            builder.Services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                    Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                    context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
                };
            });


            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

            builder.Services.AddSharedRabbitMQ(builder.Configuration);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DevTestPolicy", policy =>
                {
                    policy
                        .SetIsOriginAllowed(_ => true)   // cho phép mọi origin kể cả null (file://)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();             // bắt buộc cho SignalR
                });
            });

            builder.AddDependencies();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddHostedService<BookingCancelledConsumer>();
            builder.Services.AddHostedService<BookingEventsConsumer>();
            builder.Services.AddHostedService<SocialEventsConsumer>();

            builder.Services.AddHttpClient<IStaffService, StaffService>(
                (sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                    client.BaseAddress = new Uri(options.Auth);
                });

            builder.Services.AddHttpClient<IBookingService, BookingService>(
                (sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                    client.BaseAddress = new Uri(options.Booking);
                });

            builder.Services.AddSignalR();


            var app = builder.Build();

            app.UseCors("DevTestPolicy");

            app.UseExceptionHandler();

            app.UseSwaggerConfiguration();

            app.UseHttpsRedirection();

            app.MapHealthChecks("/health");

            app.UseAuthorization();

            app.MapHub<HousekeepingHub>("/hubs/housekeeping");

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    if (context.Database.GetPendingMigrations().Any())
                    {
                        context.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    // Log error or handle exception
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            app.Run();
        }
    }
}
