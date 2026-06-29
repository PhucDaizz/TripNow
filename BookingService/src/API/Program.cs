using BookingService.API.Extensions;
using BookingService.API.GrpcServices;
using BookingService.API.StartUp;
using BookingService.Application;
using BookingService.Infrastructure;

namespace BookingService.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCustomExceptionHandling();
            builder.Services.AddHealthChecks();

            builder.AddDependencies();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            

            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddGrpc();

            

            var app = builder.Build();

            app.UseExceptionHandler();
            app.UseSwaggerConfiguration();
            app.UseHttpsRedirection();
            app.MapHealthChecks("/health");
            app.UseAuthorization();
            app.MapGrpcService<BookingGrpcEndpoint>();
            app.MapControllers();

            await app.Services.InitializeDatabaseAsync();
        }
    }
}
