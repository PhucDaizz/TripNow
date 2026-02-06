using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Features.SettlementPeriod.Commands.RunSettlement;

namespace PaymentService.Infrastructure.BackgroundJobs
{
    public class SettlementWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<SettlementWorker> _logger;

        private readonly TimeSpan _period = TimeSpan.FromHours(1);
        public SettlementWorker(IServiceScopeFactory serviceScopeFactory, ILogger<SettlementWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Settlement Worker started running.");

            using var timer = new PeriodicTimer(_period);

            while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow;

                    //  (vd: chạy hàng ngày thì bỏ check DayOfWeek)
                    bool isRunTime = now.DayOfWeek == DayOfWeek.Monday && now.Hour == 0;

                    if (isRunTime)
                    {
                        _logger.LogInformation("Đến giờ đối soát! Đang khởi chạy job...");
                        await RunJobAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi xảy ra trong quá trình chạy Background Worker.");
                }
            }
        }

        private async Task RunJobAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                _logger.LogInformation("Worker đang gửi lệnh RunSettlementCommand...");

                var result = await mediator.Send(new RunSettlementCommand(), stoppingToken);

                if (result.IsFailure)
                {
                    _logger.LogError("Đối soát thất bại: {Error}", result.Error.Message);
                }
                else
                {
                    _logger.LogInformation("Đối soát hoàn tất thành công.");
                }
            }
        }
    }
}
