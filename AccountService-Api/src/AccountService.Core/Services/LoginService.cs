using AccountService.Core.Dtos;
using AccountService.Core.ExceptionMappers;
using AccountService.Core.Helpers;
using AccountService.Core.Interfaces;

namespace AccountService.Core.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService  _jwtTokenService;
        public LoginService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }
        public async Task<string> AuthenticateAsync(LoginRequestDto loginDto)
        {
            var user = await _userRepository.GetByUserNameAsync(loginDto.UserName);

            if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid credentials");

            return _jwtTokenService.GenerateJwtToken(user.UserId, user.UserName, user.Role);

        }
    }
}
