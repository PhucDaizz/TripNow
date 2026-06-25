using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Common;
using HotelCatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelCatalogService.Infrastructure
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IDomainEventService _domainEventService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }
        
        public DbSet<Hotel> Hotel { get; set; }
        public DbSet<Amenity> Amenity { get; set; }
        public DbSet<Block> Block { get; set; }
        public DbSet<Floor> Floor { get; set; }
        public DbSet<HotelAmenity> HotelAmenity { get; set; }
        public DbSet<Promotion> Promotion { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<RoomPrice> RoomPrice { get; set; }
        public DbSet<RoomType> RoomType { get; set; }
        public DbSet<RoomTypeImage> RoomTypeImage { get; set; }
        public DbSet<HotelImage> HotelImage { get; set; }
        public DbSet<PromotionUsage> PromotionUsage  { get; set; }
        public DbSet<CancellationPolicy> CancellationPolicy { get; set; }


        public IQueryable<Hotel> Hotels => Hotel;
        public IQueryable<Amenity> Amenities => Amenity;
        public IQueryable<Block> Blocks => Block;
        public IQueryable<Floor> Floors => Floor;
        public IQueryable<HotelAmenity> HotelAmenities => HotelAmenity;
        public IQueryable<Promotion> Promotions => Promotion;
        public IQueryable<Room> Rooms => Room;
        public IQueryable<RoomPrice> RoomPrices => RoomPrice;
        public IQueryable<RoomType> RoomTypes => RoomType;
        public IQueryable<RoomTypeImage> RoomTypeImages => RoomTypeImage;
        public IQueryable<HotelImage> HotelImages => HotelImage;
        public IQueryable<PromotionUsage> PromotionUsages => PromotionUsage;
        public IQueryable<CancellationPolicy> CancellationPolicies => CancellationPolicy;

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
