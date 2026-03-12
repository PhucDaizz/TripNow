using Microsoft.AspNetCore.Http.Features;
using Nexus.BuildingBlocks.Extensions;
using NotificationService.API.Common.ExceptionHandling;
using NotificationService.API.ExceptionHandling;
using NotificationService.API.Extensions;
using NotificationService.API.StartUp;
using NotificationService.Application;
using NotificationService.Infrastructure;
using NotificationService.Infrastructure.BackgroundJobs.Consumer;
using NotificationService.Infrastructure.Hubs;
using System.Diagnostics;

namespace NotificationService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            builder.Services.AddSharedRabbitMQ(builder.Configuration);


            builder.AddDependencies();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddHostedService<SocialNotificationConsumer>();
            builder.Services.AddHostedService<SystemNotificationConsumer>();

            var app = builder.Build();

            app.UseExceptionHandler();

            app.UseSwaggerConfiguration();

            app.UseHttpsRedirection();

            app.MapHealthChecks("/health");
            app.MapHub<NotificationHub>("/notificationhub");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
