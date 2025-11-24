using AccountService.Core.Dtos;

namespace AccountService.Core.Interfaces
{
    public interface IUserService
    {
        Task<SetRoleResponseDto> SetRole(string userName, string role);
        Task<UserResponseDto> GetByUserIdAsync(Guid userId);
    }
}
