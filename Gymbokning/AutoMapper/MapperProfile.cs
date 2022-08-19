using AutoMapper;
using Gymbokning.Models;
using Gymbokning.Models.ViewModels;

namespace Gymbokning.AutoMapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<GymClass, GymClassDetailsViewModel>()
                .ForMember(
                dest => dest.AttendingApplicationUserEmails,
                from => from.MapFrom(s => s.ApplicationUsers.ToList().Select(v => v.Email)));
        }
    }
}
