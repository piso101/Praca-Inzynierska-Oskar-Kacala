using AutoMapper;
using BackEnd.Domain.Entities;

namespace BackEnd.Application.Contracts.Auth
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<LoginRequestDto, UserAccount>()
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.PasswordHash, o => o.MapFrom(s => s.Password));
        }
    }
}



