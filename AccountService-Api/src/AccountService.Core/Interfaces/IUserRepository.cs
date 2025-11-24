using AccountService.Core.DbModels;
using AccountService.Core.Dtos;

namespace AccountService.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUserNameAsync(string userName);
        Task<User?> GetByUserIdAsync(Guid userId);
        
        Task<UserResponseDto> Register(User user);

        Task UpdateUserAsync(User user);
    }
}
