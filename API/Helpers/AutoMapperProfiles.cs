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
            CreateMap<Messages, MessageDto>()
            .ForMember(s=>s.SenderPhotoUrl, o=> o.MapFrom(u=>u.Sender.Photos
            .FirstOrDefault(x=>x.IsProfile).Url))
            .ForMember(r=>r.RecipientPhotoUrl, o=> o.MapFrom(u=>u.Sender.Photos
            .FirstOrDefault(x=>x.IsProfile).Url));

            CreateMap<DateTime, DateTime>().ConvertUsing(d=>DateTime.SpecifyKind(d, DateTimeKind.Utc));
            CreateMap<DateTime?, DateTime?>().ConvertUsing(d=>d.HasValue ? 
                                                           DateTime.SpecifyKind(d.Value, DateTimeKind.Utc)
                                                           : null);
        }
    }
}