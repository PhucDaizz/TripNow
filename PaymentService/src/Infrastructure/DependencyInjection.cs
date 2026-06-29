using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nexus.BuildingBlocks.Extensions;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Contracts;
using PaymentService.Domain.Repositories;
using PaymentService.Infrastructure.BackgroundJobs;
using PaymentService.Infrastructure.BackgroundJobs.Consumer.OwnerWallet;
using PaymentService.Infrastructure.BackgroundJobs.Consumer.Payment;
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
            services.Configure<ServiceUrlOptions>(
                configuration.GetSection(ServiceUrlOptions.SectionName));

            services.Configure<PayoutSettings>(
                configuration.GetSection(PayoutSettings.SectionName)
            );


            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseMySql(
                   configuration.GetConnectionString("DefaultConnection"),
                   new MySqlServerVersion(new Version(8, 0, 21)),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.Configure<VNPAYSettings>(
                configuration.GetSection(VNPAYSettings.SectionName));

            services.Configure<ServiceFeeSettings>(
                configuration.GetSection(ServiceFeeSettings.SectionName));

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
            services.AddScoped<IHotelCatalogService, HotelCatalogService>();

            services.AddHostedService<PaymentEventsConsumer>();
            services.AddHostedService<OwnerWalletEventsConsumer>();
            services.AddHostedService<SettlementWorker>();

            services.AddSharedRabbitMQ(configuration);

            return services;
        }

        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                if (context.Database.GetPendingMigrations().Any())
                {
                    await context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
                logger.LogError(ex, "An error occurred while migrating the database.");
            }
        }
    }
}
