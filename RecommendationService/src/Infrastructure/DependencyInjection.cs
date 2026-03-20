using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Domain.Repositories;
using RecommendationService.Infrastructure.Data.Repositories;
using RecommendationService.Infrastructure.Services;
using RecommendationService.Infrastructure.Settings;

namespace RecommendationService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<QdrantSettings>(configuration.GetSection("Qdrant"));
            services.AddSingleton<IQdrantService, QdrantService>();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseMySql(
                   configuration.GetConnectionString("DefaultConnection"),
                   new MySqlServerVersion(new Version(8, 0, 21)),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserViewedHotelRepository, UserViewedHotelRepository>();
            services.AddScoped<IUserSearchHistoryRepository, UserSearchHistoryRepository>();

            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IIntegrationEventService, IntegrationEventService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
