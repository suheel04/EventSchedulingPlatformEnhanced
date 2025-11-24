using EventService.Core.DbModels;
using EventService.Infrastructure.Database;
using EventService.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace EventService.Infrastructure.tests.Repository
{
    public class EventRepositoryTests
    {
        private EventDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<EventDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new EventDbContext(options);
        }

        private async Task SeedData(EventDbContext context)
        {
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();

            context.Events.AddRange(
                new EventItem { Id = Guid.NewGuid(), Title = "Event1", Location = "Office", Start = DateTime.UtcNow.AddDays(1), End = DateTime.UtcNow.AddDays(2), UserId = user1, CategoryId = Guid.NewGuid() },
                new EventItem { Id = Guid.NewGuid(), Title = "Event2", Location = "Home", Start = DateTime.UtcNow.AddDays(3), End = DateTime.UtcNow.AddDays(4), UserId = user1, CategoryId = Guid.NewGuid() },
                new EventItem { Id = Guid.NewGuid(), Title = "Event3", Location = "Office", Start = DateTime.UtcNow.AddDays(5), End = DateTime.UtcNow.AddDays(6), UserId = user2, CategoryId = Guid.NewGuid() },
                new EventItem { Id = Guid.NewGuid(), Title = "Event4", Location = "Business", Start = DateTime.UtcNow.AddDays(7), End = DateTime.UtcNow.AddDays(8), UserId = user2, CategoryId = Guid.NewGuid() }
            );

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task QueryAsync_FiltersByUserId_ReturnsCorrectItems()
        {
            var context = GetInMemoryDb();
            await SeedData(context);
            var repo = new EventRepository(context);

            var userId = context.Events.First().UserId;

            var (items, total) = await repo.QueryAsync(userId, null, null, null, 1, 10);

            Assert.All(items, e => Assert.Equal(userId, e.UserId));
            Assert.Equal(2, total); // user1 has 2 events
        }

        [Fact]
        public async Task QueryAsync_FiltersByAdminUserId_ReturnsCorrectItems()
        {
            var context = GetInMemoryDb();
            await SeedData(context);
            var repo = new EventRepository(context);

            var (items, total) = await repo.QueryAsync(null, null, null, null, 1, 10);//When admin userid will be passed as null

            Assert.Equal(4, total); // able to view all user events
        }

        [Fact]
        public async Task QueryAsync_FiltersByDateRange_ReturnsCorrectItems()
        {
            var context = GetInMemoryDb();
            await SeedData(context);
            var repo = new EventRepository(context);

            var from = DateTime.UtcNow.AddDays(3);
            var to = DateTime.UtcNow.AddDays(7);
            var expectedTotal = context.Events
                                .Where(e => e.Start >= from)
                                .Where(e => e.End <= to)
                                .Count();

            var (items, total) = await repo.QueryAsync(null, from, to, null, 1, 10);

            Assert.All(items, e =>
            {
                Assert.True(e.Start >= from);
                Assert.True(e.End <= to);
            });

            Assert.Equal(expectedTotal, total); // Events  fall in range
        }

        [Fact]
        public async Task QueryAsync_FiltersByLocation_ReturnsCorrectItems()
        {
            var context = GetInMemoryDb();
            await SeedData(context);
            var repo = new EventRepository(context);

            var (items, total) = await repo.QueryAsync(null, null, null, "Office", 1, 10);

            Assert.All(items, e => Assert.Contains("Office", e.Location));
            Assert.Equal(2, total);
        }

        [Fact]
        public async Task QueryAsync_Pagination_WorksCorrectly()
        {
            var context = GetInMemoryDb();
            await SeedData(context);
            var repo = new EventRepository(context);

            // page 1, size 2
            var (items, total) = await repo.QueryAsync(null, null, null, null, 1, 2);
            Assert.Equal(2, items.Count());
            Assert.Equal(4, total);

            // page 2, size 2
            var (items2, _) = await repo.QueryAsync(null, null, null, null, 2, 2);
            Assert.Equal(2, items2.Count());
        }

        [Fact]
        public async Task QueryAsync_SortingByStart_WorksCorrectly()
        {
            var context = GetInMemoryDb();
            await SeedData(context);
            var repo = new EventRepository(context);

            var (items, _) = await repo.QueryAsync(null, null, null, null, 1, 10);

            var sorted = items.OrderBy(e => e.Start).ToList();
            Assert.Equal(sorted.Select(e => e.Id), items.Select(e => e.Id));
        }

        [Fact]
        public async Task QueryAsync_AllFilters_Pagination_Sorting_WorkCorrectly()
        {
            // Arrange
            var context = GetInMemoryDb();
            await SeedData(context);
            var repo = new EventRepository(context);

            var userIdFilter = context.Events.First().UserId;
            var fromFilter = DateTime.UtcNow.AddDays(2);
            var toFilter = DateTime.UtcNow.AddDays(7);
            var locationFilter = "Office";
            int pageNumber = 1;
            int pageSize = 2;

            // Act
            var (items, total) = await repo.QueryAsync(
                userIdFilter,
                fromFilter,
                toFilter,
                locationFilter,
                pageNumber,
                pageSize
            );

            // Assert

            // 1️ Check total count matches expected after filters
            var expectedTotal = context.Events
                .Where(e => e.UserId == userIdFilter)
                .Where(e => e.Start >= fromFilter)
                .Where(e => e.End <= toFilter)
                .Where(e => e.Location.Contains(locationFilter))
                .Count();

            Assert.Equal(expectedTotal, total);

            // 2️ Check each returned item matches filters
            foreach (var e in items)
            {
                Assert.Equal(userIdFilter, e.UserId);
                Assert.True(e.Start >= fromFilter);
                Assert.True(e.End <= toFilter);
                Assert.Contains(locationFilter, e.Location);
            }

            // 3️ Check sorting by Start date
            var sorted = items.OrderBy(e => e.Start).ToList();
            Assert.Equal(sorted.Select(e => e.Id), items.Select(e => e.Id));

            // 4️ Check pagination
            Assert.True(items.Count() <= pageSize);
        }
    }
}