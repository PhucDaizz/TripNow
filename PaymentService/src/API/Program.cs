using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nexus.BuildingBlocks.Extensions;
using PaymentService.API.Common.ExceptionHandling;
using PaymentService.API.ExceptionHandling;
using PaymentService.API.Extensions;
using PaymentService.API.StartUp;
using PaymentService.Application;
using PaymentService.Application.Contracts;
using PaymentService.Infrastructure;
using PaymentService.Infrastructure.BackgroundJobs;
using PaymentService.Infrastructure.BackgroundJobs.Consumer.OwnerWallet;
using PaymentService.Infrastructure.BackgroundJobs.Consumer.Payment;
using PaymentService.Infrastructure.Services;
using PaymentService.Infrastructure.Settings;
using System.Diagnostics;

namespace PaymentService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<ServiceUrlOptions>(
                builder.Configuration.GetSection(ServiceUrlOptions.SectionName));

            builder.Services.Configure<PayoutSettings>(
                builder.Configuration.GetSection(PayoutSettings.SectionName)
            );

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

            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSharedRabbitMQ(builder.Configuration);

            builder.AddDependencies();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddHostedService<PaymentEventsConsumer>();
            builder.Services.AddHostedService<OwnerWalletEventsConsumer>();
            builder.Services.AddHostedService<SettlementWorker>();

            builder.Services.AddHttpClient<IHotelCatalogService, HotelCatalogService>(
                (sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptions<ServiceUrlOptions>>().Value;
                    client.BaseAddress = new Uri(options.HotelCatalog);
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
                    var context = services.GetRequiredService<PaymentService.Infrastructure.ApplicationDbContext>();
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
