using BookingService.API.Common.ExceptionHandling;
using BookingService.API.ExceptionHandling;
using BookingService.API.Extensions;
using BookingService.API.StartUp;
using BookingService.Application;
using BookingService.Application.Contracts;
using BookingService.Infrastructure;
using BookingService.Infrastructure.BackgroundJobs.Consumer.Inventory;
using BookingService.Infrastructure.Services;
using BookingService.Infrastructure.Settings;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nexus.BuildingBlocks.Extensions;
using System.Diagnostics;

namespace BookingService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<InventorySettings>(
               builder.Configuration.GetSection(InventorySettings.SectionName)
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

            builder.AddDependencies();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddHostedService<InventoryEventsConsumer>();

            builder.Services.AddHttpClient<IHotelCatalogService, HotelCatalogService>(
                (sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                    client.BaseAddress = new Uri(options.HotelCatalog);
                });

            builder.Services.AddHttpClient<IPaymentService, PaymentService>(
                (sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                    client.BaseAddress = new Uri(options.Payment);
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
                    var context = services.GetRequiredService<BookingService.Infrastructure.ApplicationDbContext>();
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
