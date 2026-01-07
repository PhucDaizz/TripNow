using BookingService.Application.Common.Interfaces;
using BookingService.Domain.Common;
using BookingService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IDomainEventService _domainEventService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }
        
        public DbSet<Booking> Booking { get; set; } 
        public DbSet<BookingCancellation> BookingCancellation { get; set; } 
        public DbSet<BookingItem> BookingItem { get; set; } 
        public DbSet<BookingPriceDetail> BookingPriceDetail { get; set; } 
        public DbSet<BookingPriceSnapshot> BookingPriceSnapshot { get; set; } 
        public DbSet<Inventory> Inventory { get; set; } 
        public DbSet<RoomAssignment> RoomAssignment { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // write your customizations here    
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchDomainEventsAsync(cancellationToken);
            return await base.SaveChangesAsync(cancellationToken);
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default)
        {
            var domainEntities = ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(x => x.Entity)
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.DomainEvents)
                .ToList();

            domainEntities.ForEach(entity => entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _domainEventService.PublishAsync(domainEvent, cancellationToken);
            }
        }
    }
}
