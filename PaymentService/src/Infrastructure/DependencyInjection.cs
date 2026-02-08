using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Contracts;
using PaymentService.Domain.Repositories;
using PaymentService.Infrastructure.Data.Repositories;
using PaymentService.Infrastructure.Services;
using PaymentService.Infrastructure.Settings;
using VNPAY.Extensions;

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

            services.Configure<VNPAYSettings>(
                configuration.GetSection(VNPAYSettings.SectionName));

            services.AddSingleton<IServiceFeeSettings>(sp =>
                sp.GetRequiredService<IOptions<ServiceFeeSettings>>().Value);

            var vnpayConfig = configuration.GetSection(VNPAYSettings.SectionName);

            services.AddVnpayClient(config =>
            {
                config.TmnCode = vnpayConfig["TmnCode"]!;
                config.HashSecret = vnpayConfig["HashSecret"]!;
                config.CallbackUrl = vnpayConfig["CallbackUrl"]!;
                config.BaseUrl = vnpayConfig["BaseUrl"]!;
                config.Version = vnpayConfig["Version"]!;
                config.OrderType = vnpayConfig["OrderType"]!;
            });

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());


            services.AddScoped<IEscrowAccountRepository, EscrowAccountRepository>();
            services.AddScoped<IOwnerWalletRepository, OwnerWalletRepository>();
            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
            services.AddScoped<ISettlementPeriodRepository, SettlementPeriodRepository>();
            services.AddScoped<IOwnerBankAccountRepository, OwnerBankAccountRepository>();
            services.AddScoped<IPayoutRepository, PayoutRepository>();
            services.AddScoped<IRefundRequestRepository, RefundRequestRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ISettlementService, SettlementService>();
            services.AddScoped<IPaymentService, Services.PaymentService>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddScoped<IIntegrationEventService, IntegrationEventService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
