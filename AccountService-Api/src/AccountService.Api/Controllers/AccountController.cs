using AccountService.Core.Dtos;
using AccountService.Core.ExceptionMappers;
using AccountService.Core.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace AccountService.Api.Controllers
{
    [Route("api/v{version:apiVersion}/account")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AccountController : ControllerBase
    {
        private readonly IRegisterService _registerService;
        private readonly ILoginService _loginService;
        private readonly IUserService _setRoleService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IRegisterService registerService, ILoginService loginService, IUserService setRoleService, ILogger<AccountController> logger)
        {
            _registerService = registerService;
            _loginService = loginService;
            _setRoleService = setRoleService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDtos)
        {
            try
            {
                var result = await _registerService.Register(registerRequestDtos);
                return Ok(result);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning("Validation failed: {Errors}", ex.Errors);
                return BadRequest(new ErrorResponseDto { Errors = ex.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Mesage at Register:{Message}; StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, "Something went wrong!");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                var result = await _loginService.AuthenticateAsync(loginRequestDto);
                return Ok(result);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning("Validation failed: {Errors}", ex.Errors);
                return BadRequest(new ErrorResponseDto { Errors = ex.Errors });
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning("Exception Mesage at Login:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Mesage at Login:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, "Something went wrong!");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("set-role")]
        public async Task<IActionResult> SetRole([FromQuery] string userName, [FromQuery] string role)
        {
            try
            {
                var result = await _setRoleService.SetRole(userName, role);
                return Ok(result);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning("Validation failed: {Errors}", ex.Errors);
                return BadRequest(new ErrorResponseDto { Errors = ex.Errors });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("User not found during SetRole");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Message at SetRole:{Message}; StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, "Something went wrong!");
            }
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var result = await _setRoleService.GetByUserIdAsync(id);
                return Ok(result);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning("Validation failed: {Errors}", ex.Errors);
                return BadRequest(new ErrorResponseDto { Errors = ex.Errors });
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("User not found during GetUserById");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Message at GetUserById:{Message}; StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, "Something went wrong!");
            }
        }
    }
}
