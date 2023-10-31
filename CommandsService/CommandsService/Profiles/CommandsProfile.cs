﻿using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService.Protos;

namespace CommandsService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<PlatformPublishedDTO, Platform>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));

            //CreateMap<GrpcPlatformModel, Platform>()
            //    .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.PlatformId))
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            //    .ForMember(dest => dest.Commands, opt => opt.Ignore());

        }
    }
}