using EventService.Core.DbModels;
using EventService.Core.Dtos;

namespace EventService.Core.Interfaces
{
    public interface IEventManagementService
    {
        Task<EventDto> CreateAsync(EventCreateDto eventModel);
        Task<(IEnumerable<EventDto>, int Total)> Search(Guid? loggedInUserId, string loggedInUserRole, DateTime? from, DateTime? to, string? location, int pageNumber = 1, int pageSize = 10);
        Task<EventDto?> GetByIdAsync(Guid id, Guid? userId, string userRole);
        Task<bool> UpdateAsync(Guid id, EventCreateDto eventModel, Guid? loggedInUserId, string loggedInUserRole);
        Task<bool> DeleteAsync(Guid id, Guid? loggedInUserId, string loggedInUserRole);
    }
}
