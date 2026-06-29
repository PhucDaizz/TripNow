using PaymentService.API.Extensions;
using PaymentService.API.GrpcServices;
using PaymentService.API.StartUp;
using PaymentService.Application;
using PaymentService.Infrastructure;

namespace PaymentService.API
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

            app.MapHealthChecks("/health");
            app.UseExceptionHandler();

            app.UseSwaggerConfiguration();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGrpcService<PaymentGrpcEndpoint>();

            app.MapControllers();

            await app.Services.InitializeDatabaseAsync();

            app.Run();
        }
    }
}
