using RecommendationService.API.Extensions;
using RecommendationService.API.StartUp;
using RecommendationService.Application;
using RecommendationService.Infrastructure;

namespace RecommendationService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCustomExceptionHandling();

            builder.Services.AddHealthChecks();

           
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.AddDependencies();

            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddGrpc();
            
            var app = builder.Build();

            app.UseExceptionHandler();

            app.UseSwaggerConfiguration();

            app.UseHttpsRedirection();

            app.MapHealthChecks("/health");

            app.UseAuthorization();

            app.MapControllers();

            app.MapGrpcService<GrpcServices.RagGrpcEndpoint>();

            app.Run();
        }
    }
}
