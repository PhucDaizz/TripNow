using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Domain.Repositories;
using PaymentService.Infrastructure.Data.Repositories;
using PaymentService.Infrastructure.Services;

namespace PaymentService.Infrastructure
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


            services.AddScoped<IEscrowAccountRepository, EscrowAccountRepository>();
            services.AddScoped<IOwnerWalletRepository, OwnerWalletRepository>();
            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
            services.AddScoped<ISettlementPeriodRepository, SettlementPeriodRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IIntegrationEventService, IntegrationEventService>();

            return services;
        }
    }
}
