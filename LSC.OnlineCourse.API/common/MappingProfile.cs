using AutoMapper;
using LSC.OnlineCourse.Core.Entities;
using LSC.OnlineCourse.Core.Models;

namespace LSC.OnlineCourse.API.common
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<VideoRequest, VideoRequestModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName},{src.User.LastName}"));

            CreateMap<VideoRequestModel, VideoRequest>()
                .ForMember(dest => dest.User, src => src.Ignore());
        }

    }
}
