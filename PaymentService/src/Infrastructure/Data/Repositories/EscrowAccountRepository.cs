using Microsoft.EntityFrameworkCore;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Domain.Common.Models;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;
using PaymentService.Domain.Repositories;

namespace PaymentService.Infrastructure.Data.Repositories
{
    public class EscrowAccountRepository : IEscrowAccountRepository
    {
        private readonly IApplicationDbContext _context;

        public EscrowAccountRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EscrowAccount escrowAccount, CancellationToken token = default)
        {
            await _context.EscrowAccount.AddAsync(escrowAccount, token);
        }

        public Task DeleteAsync(EscrowAccount escrowAccount, CancellationToken token = default)
        {
            _context.EscrowAccount.Remove(escrowAccount);
            return Task.CompletedTask;
        }

        public async Task<EscrowAccount?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.EscrowAccount.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public async Task<EscrowAccount?> GetByBookingIdAsync(Guid bookingId, CancellationToken token = default)
        {
            return await _context.EscrowAccount.FirstOrDefaultAsync(x => x.BookingId == bookingId, token);
        }

        public Task UpdateAsync(EscrowAccount escrowAccount, CancellationToken token = default)
        {
            _context.EscrowAccount.Update(escrowAccount);
            return Task.CompletedTask;
        }

        public async Task<PagedResult<EscrowAccount>> GetPagedListAsync(int pageNumber, int pageSize, EscrowStatus? status, Guid? bookingId, DateTime? fromDate, DateTime? toDate, CancellationToken token)
        {
            var query = _context.EscrowAccount.AsNoTracking();

            if (bookingId.HasValue)
            {
                query = query.Where(x => x.BookingId == bookingId);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status);
            }

            if (fromDate.HasValue) query = query.Where(x => x.CreatedAt >= fromDate.Value);
            if (toDate.HasValue) query = query.Where(x => x.CreatedAt <= toDate.Value);

            query = query.OrderByDescending(x => x.CreatedAt);

            var totalCount = await query.CountAsync(token);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);

            return new PagedResult<EscrowAccount>(items, totalCount, pageNumber, pageSize);
        }
    }
}
