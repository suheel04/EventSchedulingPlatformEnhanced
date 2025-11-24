using AutoMapper;
using EventService.Core.DbModels;
using EventService.Core.Dtos;

namespace EventService.Core.Mappings
{
    public class EventProfile:Profile
    {
        public EventProfile()
        {
            CreateMap<EventCreateDto, EventItem>()
                 .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
        }
    }
}
