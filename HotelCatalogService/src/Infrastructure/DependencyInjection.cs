using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Application.Contracts;
using HotelCatalogService.Domain.Repositories;
using HotelCatalogService.Infrastructure.Data.Repositories;
using HotelCatalogService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelCatalogService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseMySql(
                   configuration.GetConnectionString("DefaultConnection"),
                   new MySqlServerVersion(new Version(8, 0, 21)),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IAmenityRepository, AmenityRepository>();
            services.AddScoped<ICancellationPolicyRepository, CancellationPolicyRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IEmailServices, EmailServices>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<ICloudinaryService, CloudinaryService>();
            services.AddSingleton<IImageProcessor, ImageSharpProcessor>();

            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IIntegrationEventService, IntegrationEventService>();

            return services;
        }
    }
}
