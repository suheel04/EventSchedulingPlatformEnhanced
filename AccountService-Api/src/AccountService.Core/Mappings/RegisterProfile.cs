using AccountService.Core.DbModels;
using AccountService.Core.Dtos;
using AutoMapper;

public class RegisterProfile : Profile
{
    public RegisterProfile()
    {
        CreateMap<RegisterRequestDto, User>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.NewGuid())) // create new Guid
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password)) // hash later
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "User"))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
    }
}
