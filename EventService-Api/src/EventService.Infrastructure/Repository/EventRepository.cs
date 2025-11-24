using EventService.Core.DbModels;
using EventService.Core.Interfaces;
using EventService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EventService.Infrastructure.Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly EventDbContext _db;
        public EventRepository(EventDbContext db) { _db = db; }

        public async Task AddAsync(EventItem item)
        {
            // Check valid CategoryId
            var exists = await _db.Categories.AnyAsync(c => c.Id == item.CategoryId);
            if (!exists) throw new Exception("Invalid Category Id");
            await _db.Events.AddAsync(item);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var found = await _db.Events.FindAsync(id);
            if (found != null) { _db.Events.Remove(found); await _db.SaveChangesAsync(); }
        }

        public async Task<EventItem?> GetByIdAsync(Guid id)
        {
            return await _db.Events.FindAsync(id);
        }

        public async Task UpdateAsync(EventItem item)
        {
            _db.Events.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task<(IEnumerable<EventItem> Items, int Total)> QueryAsync(Guid? userId, DateTime? from, DateTime? to, string? location, int pageNumber, int pageSize)
        {
            var q = _db.Events.AsQueryable();

            if (userId.HasValue) q = q.Where(e => e.UserId == userId.Value);
            if (from.HasValue) q = q.Where(e => e.Start >= from.Value);
            if (to.HasValue) q = q.Where(e => e.End <= to.Value);
            if (!string.IsNullOrWhiteSpace(location)) q = q.Where(e => e.Location.Contains(location));

            var total = await q.CountAsync();
            var items = await q.OrderBy(e => e.Start)
                               .Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
            return (items, total);
        }
    }
}
