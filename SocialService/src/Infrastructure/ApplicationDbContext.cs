using Microsoft.EntityFrameworkCore;
using SocialService.Application.Common.Interfaces;
using SocialService.Domain.Common;
using SocialService.Domain.Entities;

namespace SocialService.Infrastructure
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IDomainEventService _domainEventService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<Comment> Comment { get; set; }
        public DbSet<PostLike> PostLike { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<SavedPost> SavedPost { get; set; }
        public DbSet<UserFollow> UserFollow { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<PostImage> PostImage { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Member> Member { get; set; }

        public IQueryable<Comment> Comments => Comment;
        public IQueryable<PostLike> PostLikes => PostLike;
        public IQueryable<Location> Locations => Location;
        public IQueryable<SavedPost> SavedPosts => SavedPost;
        public IQueryable<UserFollow> UserFollows => UserFollow;
        public IQueryable<Review> Reviews => Review;
        public IQueryable<PostImage> PostImages => PostImage;
        public IQueryable<Post> Posts => Post;
        public IQueryable<Member> Members => Member;


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
