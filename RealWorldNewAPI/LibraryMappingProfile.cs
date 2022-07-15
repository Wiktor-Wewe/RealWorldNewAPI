using AutoMapper;
using RealWorldNew.Common.DtoModels;
using RealWorldNew.DAL.Entities;

namespace RealWorldNewAPI
{
    public class LibraryMappingProfile : Profile
    {
        public LibraryMappingProfile()
        {
            CreateMap<RegisterUserDto, User>();
            CreateMap<User, ViewUserDto>();
        }
    }
}
