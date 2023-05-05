using AutoMapper;
using ReasonTogetherForum.Data;
using ReasonTogetherForum.Models.Account;

namespace ReasonTogetherForum
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegistrationModel, ApplicationUser>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
