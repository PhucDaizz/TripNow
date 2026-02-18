using BookingService.Application.Common.Interfaces;
using BookingService.Domain.Repositories;
using BookingService.Infrastructure.Data.Repositories;
using BookingService.Infrastructure.Services;
using BookingService.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BookingService.Infrastructure
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

            services.AddScoped<IInventorySettings>(sp =>
                sp.GetRequiredService<IOptions<InventorySettings>>().Value);


            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingPriceSnapshotRepository, BookingPriceSnapshotRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IInventoryConfigurationRepository, InventoryConfigurationRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IIntegrationEventService, IntegrationEventService>();

            return services;
        }
    }
}
