using ChatService.API.Common.ExceptionHandling;
using ChatService.API.ExceptionHandling;
using ChatService.API.Extensions;
using ChatService.API.StartUp;
using ChatService.Application;
using ChatService.Infrastructure;
using ChatService.Infrastructure.Hubs;
using Microsoft.AspNetCore.Http.Features;
using Nexus.BuildingBlocks.Extensions;
using System.Diagnostics;

namespace ChatService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            // CORS — cho phép tất cả origin khi dev/test (bao gồm file://)
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
            var app = builder.Build();

            app.UseExceptionHandler();

            app.UseSwaggerConfiguration();

            app.UseHttpsRedirection();

            app.UseCors("DevTestPolicy");        // phải đứng trước UseAuthentication

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHealthChecks("/health");
            app.MapHub<ChatHub>("/chathub");


            app.MapControllers();

            app.Run();
        }
    }
}
