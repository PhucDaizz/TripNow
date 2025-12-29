using DotNetEnv;
using HotelCatalogService.API.Common.ExceptionHandling;
using HotelCatalogService.API.ExceptionHandling;
using HotelCatalogService.API.Extensions;
using HotelCatalogService.API.StartUp;
using HotelCatalogService.Application;
using HotelCatalogService.Application.Contracts;
using HotelCatalogService.Infrastructure;
using HotelCatalogService.Infrastructure.Services;
using HotelCatalogService.Infrastructure.Settings;
using Microsoft.AspNetCore.Http.Features;
using Nexus.BuildingBlocks.Extensions;
using System.Diagnostics;

namespace HotelCatalogService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load($"../Config/.env");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<CloudinarySettings>(
               builder.Configuration.GetSection(CloudinarySettings.SectionName)
            );
            builder.Services.Configure<EmailSettings>(
                builder.Configuration.GetSection(EmailSettings.SectionName)
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


            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

            builder.Services.AddSharedRabbitMQ(builder.Configuration);

            builder.AddDependencies();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddHttpClient<IStaffService, StaffService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:7001"); 
            });


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
