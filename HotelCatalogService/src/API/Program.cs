using HotelCatalogService.API.Extensions;
using HotelCatalogService.API.StartUp;
using HotelCatalogService.Application;
using HotelCatalogService.Infrastructure;
using HotelCatalogService.Infrastructure.Hubs;

namespace HotelCatalogService.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Đăng ký Exception Handling
            builder.Services.AddCustomExceptionHandling();
            builder.Services.AddHealthChecks();

            // 2. Đăng ký các Layer 
            builder.AddDependencies();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

            builder.Services.AddHttpContextAccessor();
            
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

            var app = builder.Build();

            app.UseCors("DevTestPolicy");
            app.UseExceptionHandler();
            app.UseSwaggerConfiguration();
            app.UseHttpsRedirection();
            app.MapHealthChecks("/health");
            app.UseAuthorization();
            app.MapHub<HousekeepingHub>("/hubs/housekeeping");
            app.MapControllers();

            await app.InitializeDatabaseAsync();

            app.Run();
        }
    }
}
