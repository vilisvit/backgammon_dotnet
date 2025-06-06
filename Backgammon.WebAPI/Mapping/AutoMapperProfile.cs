using AutoMapper;
using Backgammon.Core.Entities;
using Backgammon.GameCore.Game;
using Backgammon.WebAPI.DTOs.Board;
using Backgammon.WebAPI.DTOs.Comment;
using Backgammon.WebAPI.DTOs.Score;

namespace Backgammon.WebAPI.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Comment, CommentResponseDto>()
            .ForMember(dest => dest.Player, opt => opt.MapFrom(src => src.User.UserName))
            // .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id)) TODO: Uncomment after frontend update
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Content)); // TODO: remove this one after frontend update

        CreateMap<Score, ScoreResponseDto>()
            .ForMember(dest => dest.Player, opt => opt.MapFrom(src => src.User.UserName));
        // .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id)) TODO: Uncomment after frontend update
        
        CreateMap<Board, BoardDto>().ConvertUsing<BoardToDtoConverter>();
    }
}