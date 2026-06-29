using SocialService.API.Extensions;
using SocialService.API.StartUp;
using SocialService.Application;
using SocialService.Infrastructure;

namespace SocialService.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCustomExceptionHandling();
            builder.Services.AddHealthChecks();

            builder.Services.AddHttpContextAccessor();

            builder.AddDependencies();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

            var app = builder.Build();

            app.UseExceptionHandler();

            app.UseSwaggerConfiguration();

            app.UseHttpsRedirection();

            app.MapHealthChecks("/health");

            app.UseAuthorization();


            app.MapControllers();

            await app.InitializeDatabaseAsync();

            app.Run();
        }
    }
}
