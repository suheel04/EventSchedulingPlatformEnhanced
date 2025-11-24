using AccountService.Core.Dtos;

namespace AccountService.Core.Interfaces
{
    public interface ILoginService
    {
        Task<string> AuthenticateAsync(LoginRequestDto loginDto);
    }
}
