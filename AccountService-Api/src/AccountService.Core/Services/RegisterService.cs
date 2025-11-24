using AccountService.Core.DbModels;
using AccountService.Core.Dtos;
using AccountService.Core.ExceptionMappers;
using AccountService.Core.Helpers;
using AccountService.Core.Interfaces;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AccountService.Core.Services
{
    public class RegisterService : IRegisterService
    {

        private readonly IUserRepository _userRespository;
        private readonly IValidator<RegisterRequestDto> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger<RegisterService> _logger;
        public RegisterService(IUserRepository userRespository, IValidator<RegisterRequestDto> validator, IMapper mapper, ILogger<RegisterService> logger)
        {
            _userRespository = userRespository;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserResponseDto> Register(RegisterRequestDto registerRequestDtos)
        {
            _logger.LogInformation("Validate Register Request Dto");
            if (registerRequestDtos is null)
                throw new BadRequestException(new[] { "Request body cannot be null" });
            var validateRequest = _validator.Validate(registerRequestDtos);
            if (!validateRequest.IsValid)
            {
                var errors = validateRequest.Errors.Select(e => e.ErrorMessage);
                throw new BadRequestException(errors);
            }
            _logger.LogInformation("Map Register Request Dto to Db Model.Username:{username}; Email:{Email}", registerRequestDtos.UserName, MaskingHelper.MaskEmail(registerRequestDtos.Email!));
            var user = _mapper.Map<User>(registerRequestDtos);
            user.PasswordHash = PasswordHasher.HashPassword(registerRequestDtos.Password!);
            var result = await _userRespository.Register(user);
            return result;
        }

    }
}
