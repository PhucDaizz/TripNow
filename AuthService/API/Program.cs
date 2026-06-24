using API.Extensions;
using API.StartUp;
using Application;
using CarbonTC.API.Extensions;
using Infrastructure;

namespace API
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

            // 3. Đăng ký Bảo mật & Authentication
            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

            var app = builder.Build();

            // 4. Khởi tạo Database (Migration & Seeding)
            await app.InitializeDatabaseAsync();

            // 5. Cấu hình HTTP Request Pipeline (Middleware)
            app.UseExceptionHandler();
            app.UseForwardedHeaders();
            app.UseSwaggerConfiguration();

            // app.UseHttpsRedirection();

            app.Use((context, next) =>
            {
                context.Request.Scheme = "http";
                context.Request.Host = new HostString("localhost", 7000); 
                return next();
            });

            app.MapHealthChecks("/health"); 

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
