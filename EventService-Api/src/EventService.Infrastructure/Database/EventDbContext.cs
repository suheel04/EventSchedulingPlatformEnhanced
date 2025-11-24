using EventService.Core.DbModels;
using Microsoft.EntityFrameworkCore;

namespace EventService.Infrastructure.Database
{
    public class EventDbContext : DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext> options) : base(options) { }
        public DbSet<EventItem> Events { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Relationship: Category 1 → Many Events
            modelBuilder.Entity<EventItem>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Category data for in-memory DB
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Home" },
                new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Personal" },
                new Category { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Office" },
                new Category { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Business" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
