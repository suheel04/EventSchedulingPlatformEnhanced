using EventService.Core.Dtos;
using FluentValidation;

namespace EventService.Core.Validation
{
    public class EventCreateDtoValidator:AbstractValidator<EventCreateDto>
    {
        public EventCreateDtoValidator()
        {
            // Title Required + Minimum length
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MinimumLength(3).WithMessage("Title must be at least 3 characters long");

            // Location Required
            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required");

            RuleFor(x => x.Start)
            .Must(date => date >= DateTime.UtcNow.Date)
            .WithMessage("Start date cannot be in the past");

            RuleFor(x => x.End)
                .GreaterThan(x => x.Start)
                .WithMessage("End must be after Start");

            // UserId must exist
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId is required");

            // Category validation
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("CategoryId is required");
        }
    }
}
