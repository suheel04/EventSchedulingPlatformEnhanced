using EventService.Core.DbModels;
using EventService.Core.Dtos;
using EventService.Core.ExceptionMappers;
using EventService.Core.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EventService.Core.Service
{
    public class EventManagementService : IEventManagementService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IValidator<EventCreateDto> _validator;
        //private readonly IMapper _mapper;TBD
        private readonly ILogger<EventManagementService> _logger;
        private readonly IAccountApi _accountApi;
        public EventManagementService(IEventRepository eventRepository, IValidator<EventCreateDto> validator, ILogger<EventManagementService> logger, IAccountApi accountApi)
        {
            _eventRepository = eventRepository;
            _validator = validator;
           // _mapper = mapper;
            _logger = logger;
            _accountApi = accountApi;
        }
        public async Task<EventDto> CreateAsync(EventCreateDto eventModel)
        {

            var validateRequest = _validator.Validate(eventModel);
            if (!validateRequest.IsValid)
            {
                var errors = validateRequest.Errors.Select(e => e.ErrorMessage);
                throw new BadRequestException(errors);
            }
            var response = await _accountApi.GetUserById(eventModel.UserId);

            if (!response.IsSuccessStatusCode)
                throw new Exception("User not found - cannot create event");

            var item = new EventItem
                {
                    Id = Guid.NewGuid(),
                    Title = eventModel.Title,
                    Location = eventModel.Location,
                    Start = eventModel.Start,
                    End = eventModel.End,
                    UserId = eventModel.UserId,
                    CategoryId=eventModel.CategoryId
                };

                await _eventRepository.AddAsync(item);
                return new EventDto { Id = item.Id, Title = item.Title, Location = item.Location, Start = item.Start, End = item.End, UserId = item.UserId,CategoryId=item.CategoryId };
        }

        public async Task<(IEnumerable<EventDto>, int Total)> Search(Guid? loggedInUserId, string loggedInUserRole, DateTime? from, DateTime? to, string? location, int pageNumber = 1, int pageSize = 10)
        {
            if (loggedInUserRole == "Admin") loggedInUserId = null;
            var (items, total) = await _eventRepository.QueryAsync(loggedInUserId, from, to, location, pageNumber, pageSize);
            var dtoList = items.Select(e => new EventDto { Id = e.Id, Title = e.Title, Location = e.Location, Start = e.Start, End = e.End, UserId = e.UserId,CategoryId=e.CategoryId });
            return (dtoList, total);
        }

        public async Task<EventDto?> GetByIdAsync(Guid id, Guid? loggedInUserId, string loggedInUserRole)
        {
            var entity = await GetEntityOrThrowNotFoundAsync(id);
            EnsureHasAccess(loggedInUserRole, loggedInUserId, entity!.UserId);

            return new EventDto { Id = entity.Id, Title = entity.Title, Location = entity.Location, Start = entity.Start, End = entity.End, UserId = entity.UserId, CategoryId = entity.CategoryId };            
        }


        public async Task<bool> UpdateAsync(Guid id, EventCreateDto dto, Guid? loggedInUserId, string loggedInUserRole)
        {
            var entity = await GetEntityOrThrowNotFoundAsync(id);
            EnsureHasAccess(loggedInUserRole, loggedInUserId, entity!.UserId);

            entity!.Title = dto.Title;
            entity.Location = dto.Location;
            entity.Start = dto.Start;
            entity.End = dto.End;
            entity.UserId = dto.UserId;
            entity.CategoryId = dto.CategoryId;

            await _eventRepository.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid? loggedInUserId, string loggedInUserRole)
        {
            var entity = await GetEntityOrThrowNotFoundAsync(id);
            EnsureHasAccess(loggedInUserRole, loggedInUserId, entity!.UserId);

            await _eventRepository.DeleteAsync(id);
            return true;
        }
        private async Task<EventItem> GetEntityOrThrowNotFoundAsync(Guid id)
        {
            var entity = await _eventRepository.GetByIdAsync(id);

            if (entity is null)
                throw new NotFoundException("No matching record found");

            return entity;
        }

        private static void EnsureHasAccess(string role, Guid? loggedInUserId, Guid ownerId)
        {
            if (role != "Admin" && loggedInUserId != ownerId)
                throw new ForbiddenException("You do not have permission to access this resource");
        }
    }
}
