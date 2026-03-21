using Microsoft.AspNetCore.Http.Features;
using Nexus.BuildingBlocks.Extensions;
using RecommendationService.API.Common.ExceptionHandling;
using RecommendationService.API.ExceptionHandling;
using RecommendationService.API.Extensions;
using RecommendationService.API.StartUp;
using RecommendationService.Application;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Infrastructure;
using RecommendationService.Infrastructure.Services;
using RecommendationService.Infrastructure.Settings;
using System.Diagnostics;

namespace RecommendationService.API
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
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSharedRabbitMQ(builder.Configuration);

            builder.Services.Configure<OllamaSettings>(builder.Configuration.GetSection("Ollama"));
            builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetSection("OpenAI"));

            var aiProvider = builder.Configuration["AI_Provider"] ?? "Ollama";

            if (aiProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
            {
                builder.Services.AddSingleton<IEmbeddingService, OpenAiEmbeddingService>();
            }
            else
            {
                builder.Services.AddHttpClient<OllamaEmbeddingService>();
                builder.Services.AddSingleton<IEmbeddingService, OllamaEmbeddingService>();
            }

            builder.AddDependencies();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            var app = builder.Build();

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
