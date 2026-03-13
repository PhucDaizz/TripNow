using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Nexus.BuildingBlocks.Extensions;
using SocialService.API.Common.ExceptionHandling;
using SocialService.API.ExceptionHandling;
using SocialService.API.Extensions;
using SocialService.API.StartUp;
using SocialService.Application;
using SocialService.Application.Contracts;
using SocialService.Infrastructure;
using SocialService.Infrastructure.BackgroundJobs.Consumer;
using SocialService.Infrastructure.Services;
using SocialService.Infrastructure.Settings;
using System.Diagnostics;

namespace SocialService.API
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

            builder.Services.Configure<ServiceUrlOptions>(
                builder.Configuration.GetSection(ServiceUrlOptions.SectionName));
            builder.Services.Configure<CloudinarySettings>(
               builder.Configuration.GetSection(CloudinarySettings.SectionName)
            );

            builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddHealthChecks();

            builder.Services.AddHttpContextAccessor();

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

            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

            builder.Services.AddSharedRabbitMQ(builder.Configuration);

            builder.AddDependencies();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddHostedService<MemberEventsConsumer>();
            builder.Services.AddHostedService<HotelEventsConsumer>();

            builder.Services.AddHttpClient<IHotelCatalogService, HotelCatalogService>(
               (sp, client) =>
               {
                   var options = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                   client.BaseAddress = new Uri(options.HotelCatalog);
               });
            builder.Services.AddHttpClient<IAuthService, AuthService>(
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

            var app = builder.Build();

            app.UseExceptionHandler();

            app.UseSwaggerConfiguration();

            app.UseHttpsRedirection();

            app.MapHealthChecks("/health");

            app.UseAuthorization();


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<SocialService.Infrastructure.ApplicationDbContext>();
                    if (context.Database.GetPendingMigrations().Any())
                    {
                        context.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            app.Run();
        }
    }
}
