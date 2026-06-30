using ChatService.API.Extensions;
using ChatService.API.StartUp;
using ChatService.Application;
using ChatService.Infrastructure;
using ChatService.Infrastructure.Hubs;

namespace ChatService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCustomExceptionHandling();
            builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
            
            builder.AddDependencies();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddHealthChecks();
            builder.Services.AddHttpContextAccessor();

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

            var app = builder.Build();

            app.UseExceptionHandler();

            app.UseSwaggerConfiguration();

            app.UseHttpsRedirection();

            app.UseCors("DevTestPolicy");        

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHealthChecks("/health");
            app.MapHub<ChatHub>("/chathub");


            app.MapControllers();

            app.Run();
        }
    }
}
