using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Nexus.BuildingBlocks.Extensions;
using NotificationService.API.Common.ExceptionHandling;
using NotificationService.API.ExceptionHandling;
using NotificationService.API.Extensions;
using NotificationService.API.StartUp;
using NotificationService.Application;
using NotificationService.Infrastructure;
using NotificationService.Infrastructure.Hubs;

namespace NotificationService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCustomExceptionHandling();
            builder.Services.AddHealthChecks();
            builder.Services.AddHttpContextAccessor();
           
            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

            builder.AddDependencies();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

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
