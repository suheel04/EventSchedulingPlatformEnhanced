using Asp.Versioning;
using EventService.Core.Dtos;
using EventService.Core.ExceptionMappers;
using EventService.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventService.Api.Controllers
{
    [Route("api/v{version:apiVersion}/event")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,User")]
    public class EventsController : ControllerBase
    {
        private readonly IEventManagementService _eventManagementService;
        private readonly ILogger<EventsController> _logger;
        public EventsController(IEventManagementService eventManagementService, ILogger<EventsController> logger)
        {
            _eventManagementService = eventManagementService;
            _logger = logger;
        }

        // POST api/v1/events
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EventCreateDto eventCreateDto)
        {
            try
            {
                var result = await _eventManagementService.CreateAsync(eventCreateDto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning("Validation failed at Event Create: {Errors}", ex.Errors);
                return BadRequest(new ErrorResponseDto { Errors = ex.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Mesage at Event Create:{Message}; StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, "Something went wrong!");
            }

        }

        // Search endpoint: GET api/events/search?from=2025-01-01&to=2025-02-01&location=Delhi&pageNumber=1&pageSize=10
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string? location, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (loggedInUserId, loggedInUserRole) = GetLoggedInUserInfo();
                var (dtoList, total) = await _eventManagementService.Search(loggedInUserId, loggedInUserRole, from, to, location, pageNumber, pageSize);
                return Ok(new { total, pageNumber, pageSize, items = dtoList });
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning("Exception Mesage at Event Search:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return Unauthorized(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning("Validation failed at Event Search: {Errors}", ex.Errors);
                return BadRequest(new ErrorResponseDto { Errors = ex.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Mesage at Event Search:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, "Something went wrong!");
            }
        }

        // GET api/v1/events/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var (loggedInUserId, loggedInUserRole) = GetLoggedInUserInfo();
                var result = await _eventManagementService.GetByIdAsync(id, loggedInUserId, loggedInUserRole);
                return Ok(result);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogWarning("Exception Mesage at Event GetById:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status403Forbidden, "Don't have access");
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning("Exception Mesage at Event GetById:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return Unauthorized(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning("Validation failed at Event GetById: {Errors}", ex.Errors);
                return BadRequest(new ErrorResponseDto
                {
                    Errors = ex.Errors
                });
            }
            catch(NotFoundException ex)
            {
                _logger.LogWarning("Exception Mesage at Event GetById:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Mesage at Event GetById:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, "Something went wrong!");
            }
        }

        // PUT api/events/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] EventCreateDto dto)
        {
            try
            {
                var (loggedInUserId, loggedInUserRole) = GetLoggedInUserInfo();
                var result = await _eventManagementService.UpdateAsync(id, dto, loggedInUserId, loggedInUserRole);
                return NoContent();
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning("Exception Mesage at Event Update:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return Unauthorized(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                _logger.LogWarning("Exception Mesage at Event Update:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status403Forbidden, "Don't have access");
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Exception Mesage at Event GetById:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning("Validation failed at Event Update: {Errors}", ex.Errors);
                return BadRequest(new ErrorResponseDto
                {
                    Errors = ex.Errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception Mesage at Event Update:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, "Something went wrong!");
            }
        }

       // DELETE api/events/{id}
       [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var (loggedInUserId, loggedInUserRole) = GetLoggedInUserInfo();
                var result = await _eventManagementService.DeleteAsync(id, loggedInUserId, loggedInUserRole);
                return NoContent();
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning("Exception Mesage at Event Delete:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return Unauthorized(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                _logger.LogWarning("Exception Mesage at Event Delete:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return Forbid(ex.Message);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Exception Mesage at Event GetById:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return NotFound(new { message = ex.Message });
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning("Validation failed at Event Delete: {Errors}", ex.Errors);
                return BadRequest(new ErrorResponseDto
                {
                    Errors = ex.Errors
                });
            }           
            catch (Exception ex)
            {
                _logger.LogError("Exception Mesage at Event Delete:{Message} StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                return StatusCode(500, "Something went wrong!");
            }
        }

        private (Guid? userId, string userRole) GetLoggedInUserInfo()
        {
            if (User?.Identity == null || !User.Identity.IsAuthenticated)
                throw new UnauthorizedException("User is not authenticated.");
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            var loggedInUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
            Guid? userId = null;
            if (Guid.TryParse(loggedInUserId, out var parsed)) userId = parsed;
            return (userId, loggedInUserRole);
        }
    }
}
