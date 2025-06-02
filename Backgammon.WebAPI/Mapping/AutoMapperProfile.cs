using AutoMapper;
using Backgammon.Core.Entities;
using Backgammon.WebAPI.DTOs;

namespace Backgammon.WebAPI.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Comment, CommentResponseDto>()
            .ForMember(dest => dest.Player, opt => opt.MapFrom(src => src.User.Username))
            // .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id)) TODO: Uncomment after frontend update
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Content)); // TODO: remove this one after frontend update
    }
}