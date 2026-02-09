using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MMLib.SwaggerForOcelot.DependencyInjection;

namespace ApiGateway
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
                .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerForOcelot(builder.Configuration);

            builder.Services.AddOcelot(builder.Configuration);
            
            // Add CORS if needed (e.g. for frontend development)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerForOcelotUI(opt =>
                {
                    opt.PathToSwaggerGenerator = "/swagger/docs";
                });
            }

            app.UseHttpsRedirection();
            
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            
            await app.UseOcelot();

            app.Run();
        }
    }
}
