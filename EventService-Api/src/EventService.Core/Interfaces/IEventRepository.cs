using EventService.Core.DbModels;

namespace EventService.Core.Interfaces
{
    public interface IEventRepository
    {
        Task<EventItem?> GetByIdAsync(Guid id);
        Task AddAsync(EventItem item);
        Task UpdateAsync(EventItem item);
        Task DeleteAsync(Guid id);
        Task<(IEnumerable<EventItem> Items, int Total)> QueryAsync(Guid? userId, DateTime? from, DateTime? to, string? location, int pageNumber, int pageSize);
    }
}
