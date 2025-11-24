using AccountService.Core.Dtos;
using FluentValidation;

namespace AccountService.Core.Validation
{
    public class RegisterDtoValidator: AbstractValidator<RegisterRequestDto>
    {
        public RegisterDtoValidator() 
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username is Null or Empty");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is Null or Empty");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Password is Null or Empty");
        }
    }
}
