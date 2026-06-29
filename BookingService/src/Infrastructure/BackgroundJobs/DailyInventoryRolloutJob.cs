using BookingService.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookingService.Infrastructure.BackgroundJobs
{
    public class DailyInventoryRolloutJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DailyInventoryRolloutJob> _logger;

        public DailyInventoryRolloutJob(IServiceScopeFactory scopeFactory, ILogger<DailyInventoryRolloutJob> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessDailyRollout(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi chạy job tạo inventory hàng ngày.");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task ProcessDailyRollout(CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var inventorySettings = scope.ServiceProvider.GetRequiredService<IInventorySettings>();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var maxTargetDate = today.AddDays(inventorySettings.LookAheadDays); // Ngày thứ ... cần tạo

            // 1. Lấy danh sách các RoomType ĐANG HOẠT ĐỘNG (IsActive = true)
            var activeConfigs = await unitOfWork.InventoryConfiguration.GetActiveConfigurationsAsync(token);

            var allNewInventories = new List<Domain.Entities.Inventory>();

            foreach (var config in activeConfigs)
            {
                DateOnly startDate;
                if (config.LastGeneratedDate.HasValue)
                {
                    startDate = DateOnly.FromDateTime(config.LastGeneratedDate.Value).AddDays(1);
                }
                else
                {
                    startDate = today;
                }

                // Nếu ngày bắt đầu > ngày đích -> Nghĩa là đã tạo đủ rồi -> Bỏ qua
                if (startDate > maxTargetDate) continue;

                // Lấy Stock mẫu để kế thừa (Lấy của ngày liền trước ngày bắt đầu)
                // Nếu server tắt 5 ngày, ta lấy stock của ngày thứ 6 trước đó làm chuẩn
                var previousDate = startDate.AddDays(-1);
                var lastKnownInventory = await unitOfWork.Inventory.GetLastKnownInventoryAsync(config.RoomTypeId, previousDate, token);

                int currentStockToUse = lastKnownInventory != null
                    ? lastKnownInventory.TotalStock
                    : config.DefaultStock;

                var currentDateIterator = startDate;

                while (currentDateIterator <= maxTargetDate)
                {
                    var newInv = Domain.Entities.Inventory.Create(config.RoomTypeId, currentDateIterator, currentStockToUse);
                    allNewInventories.Add(newInv);

                    // Tăng ngày lên
                    currentDateIterator = currentDateIterator.AddDays(1);
                }

                // Cập nhật mốc mới nhất vào Config (Chính là maxTargetDate)
                config.UpdateLastGeneratedDate(maxTargetDate);
            }

            if (allNewInventories.Any())
            {
                // Chia nhỏ batch nếu quá lớn (ví dụ server tắt 1 năm -> tạo 365 * n phòng)
                await unitOfWork.Inventory.AddRangeAsync(allNewInventories, token);
                await unitOfWork.SaveChangesAsync(token);

                _logger.LogInformation($"Đã chạy Backfill và tạo mới {allNewInventories.Count} inventory (Bù đắp khoảng thời gian server tắt).");
            }
        }
    }
}
