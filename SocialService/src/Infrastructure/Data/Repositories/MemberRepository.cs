using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Entities;
using SocialService.Domain.Repositories;

namespace SocialService.Infrastructure.Data.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly IApplicationDbContext _context;

        public MemberRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Member member, CancellationToken token = default)
        {
            await _context.Members.AddAsync(member, token);
        }

        public Task DeleteAsync(Member member, CancellationToken token = default)
        {
            _context.Members.Remove(member);
            return Task.CompletedTask;
        }

        public async Task<Member?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.Members.FirstOrDefaultAsync(x => x.Id == id, token);
        }

        public async Task<bool> IsExistingAsync(Guid id, CancellationToken token = default)
        {
            return await _context.Members.AnyAsync(x => x.Id == id, token);
        }

        public Task UpdateAsync(Member member, CancellationToken token = default)
        {
            _context.Members.Update(member);
            return Task.CompletedTask;
        }
    }
}
