using EventService.Core.Dtos;
using Refit;

namespace EventService.Core.Interfaces
{
    public interface IAccountApi
    {
        [Get("/api/v1/account/{id}")]
        Task<ApiResponse<UserResponseDto>> GetUserById(Guid id);
    }
}
