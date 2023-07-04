using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
            .ForMember(dest=>dest.PhotoUrl, 
                       opt=>opt.MapFrom(
                       src=>src.Photos.FirstOrDefault(
                       x=>x.IsProfile).Url))
            .ForMember(dest => dest.Age, opt =>opt.MapFrom(src =>src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<Blog, BlogDto>();
            CreateMap<Post, PostDto>();

            CreateMap<UpdateMemberDTO, AppUser>();

            CreateMap<RegisterDto, AppUser>();

        }
    }
}