using AccountService.Core.Dtos;
using AccountService.Core.ExceptionMappers;
using AccountService.Core.Interfaces;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AccountService.Core.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<RegisterService> _logger;
        private readonly IValidator<RegisterRequestDto> _validator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(ILogger<RegisterService> logger, IValidator<RegisterRequestDto> validator, IUserRepository userRepository, IMapper mapper)
        {
            _logger = logger;
            _validator = validator;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<SetRoleResponseDto> SetRole(string userName, string role)
        {
            _logger.LogInformation("Validate UserName:{userName} and Role:{role}", userName, role);
            //TBD:Use fluent validation here

            var user = await _userRepository.GetByUserNameAsync(userName);

            if (user == null) throw new NotFoundException("User not found");

            await _userRepository.UpdateUserAsync(user);

            //TBD:Use mapper here.
            return new SetRoleResponseDto
            {
                UserName = user.UserName,
                NewRole = role
            };
        }


        public async Task<UserResponseDto> GetByUserIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByUserIdAsync(userId);

            if (user == null) throw new NotFoundException("User not found");
            return new UserResponseDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserEmail = user.Email
            };
        }
    }
}
