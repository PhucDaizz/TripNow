using BookingService.Application.Common.Interfaces;
using BookingService.Application.Features.Booking.Commands.CancelBooking;
using BookingService.Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookingService.Infrastructure.BackgroundJobs
{
    public class BookingCleanupWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BookingCleanupWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var timeoutThreshold = DateTime.UtcNow.AddMinutes(-15);

                    var expiredBookings = await unitOfWork.Booking.GetExpiredPendingBookingsAsync(timeoutThreshold, stoppingToken);

                    foreach (var booking in expiredBookings)
                    {

                        await mediator.Send(
                            new CancelBookingCommand
                            {
                                BookingId = booking.Id,
                                CancelledBy = CancelledBy.System,
                                Reason = "Payment Timeout",
                                RefundPolicy = RefundPolicy.NonRefundable,
                                RefundAmount = 0
                            }, stoppingToken);
                    }

                    if (expiredBookings.Any())
                    {
                        await unitOfWork.SaveChangesAsync(stoppingToken);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
