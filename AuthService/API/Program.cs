using API.StartUp;
using Application;
using CarbonTC.API.Common.ExceptionHandling;
using CarbonTC.API.ExceptionHandling;
using CarbonTC.API.Extensions;
using Infrastructure;
using Infrastructure.BackgroundJobs.Consumer.User;
using Infrastructure.Persistence.SeedData;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Http.Features;
using Nexus.BuildingBlocks.Extensions;
using System.Diagnostics;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<CloudinarySettings>(
                builder.Configuration.GetSection(CloudinarySettings.SectionName)
            );
            builder.Services.Configure<AdminAccountOptions>(
                builder.Configuration.GetSection(AdminAccountOptions.SectionName)
            );
            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection(JwtSettings.SectionName)
            );
            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection(EmailSettings.SectionName)
            );
            builder.Services.Configure<GoogleSettings>(
                builder.Configuration.GetSection(GoogleSettings.SectionName)
            );
            builder.Services.Configure<FrontendSettings>(
                builder.Configuration.GetSection(FrontendSettings.SectionName)
            );

            builder.AddDependencies();

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

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddSharedRabbitMQ(builder.Configuration);

            builder.Services.AddHostedService<UserEventsConsumer>();


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var initializer = services.GetRequiredService<IDbInitializer>();
                    await initializer.SeedAsync(); 
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Loi khi tai du lieu.");
                }
            }
            app.UseExceptionHandler();

            app.UseSwaggerConfiguration();

            app.UseHttpsRedirection();

            app.MapHealthChecks("/health"); 

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
