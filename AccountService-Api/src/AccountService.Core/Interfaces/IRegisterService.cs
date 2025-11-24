using AccountService.Core.Dtos;

namespace AccountService.Core.Interfaces
{
    public interface IRegisterService
    {
        Task<UserResponseDto> Register(RegisterRequestDto RegisterResponseDtos);
    }
}
