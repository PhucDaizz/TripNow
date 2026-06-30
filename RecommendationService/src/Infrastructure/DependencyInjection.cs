using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexus.BuildingBlocks.Extensions;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Domain.Repositories;
using RecommendationService.Infrastructure.BackgroundJobs.Consumer;
using RecommendationService.Infrastructure.Data.Repositories;
using RecommendationService.Infrastructure.Services;
using RecommendationService.Infrastructure.Settings;

namespace RecommendationService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<OllamaSettings>(configuration.GetSection("Ollama"));
            services.Configure<OpenAiSettings>(configuration.GetSection("OpenAI"));
            services.Configure<QdrantSettings>(configuration.GetSection("Qdrant"));

            var aiProvider = configuration["AI_Provider"] ?? "Ollama";

            if (aiProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
            {
                services.AddSingleton<IEmbeddingService, OpenAiEmbeddingService>();
            }
            else
            {
                services.AddHttpClient<OllamaEmbeddingService>();
                services.AddSingleton<IEmbeddingService, OllamaEmbeddingService>();
            }


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

            // Background consumers
            services.AddHostedService<UserViewedHotelConsumer>();
            services.AddHostedService<HotelIndexedConsumer>();

            services.AddSharedRabbitMQ(configuration);

            return services;
        }
    }
}
