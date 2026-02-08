using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Repositories;

namespace PaymentService.Infrastructure.Data.Repositories
{
    public class RefundRequestRepository : IRefundRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public RefundRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefundRequest refundRequest, CancellationToken token = default)
        {
            await _context.RefundRequest.AddAsync(refundRequest);
        }

        public Task DeleteAsync(RefundRequest refundRequest, CancellationToken token = default)
        {
            _context.RefundRequest.Remove(refundRequest);
            return Task.CompletedTask;
        }

        public async Task<RefundRequest?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.RefundRequest.FirstOrDefaultAsync(rr => rr.Id == id, token);
        }

        public Task UpdateAsync(RefundRequest refundRequest, CancellationToken token = default)
        {
            _context.RefundRequest.Update(refundRequest);
            return Task.CompletedTask;
        }
    }
}
