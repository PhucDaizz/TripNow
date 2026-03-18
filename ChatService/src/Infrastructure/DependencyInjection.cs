using ChatService.Application.Common.Interfaces;
using ChatService.Application.Interface;
using ChatService.Domain.Repositories;
using ChatService.Infrastructure.Data.Repositories;
using ChatService.Infrastructure.Services;
using ChatService.Infrastructure.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.Infrastructure
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

            services.AddSignalR();

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IChatProfileRepository, ChatProfileRepository>();

            services.AddTransient<IChatNotificationService, ChatNotificationService>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IIntegrationEventService, IntegrationEventService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IHotelAuthorizationService, HotelAuthorizationService>();
            //services.AddSingleton<IChatNotificationService, ChatNotificationService>();
            return services;
        }
    }
}
