using AutoMapper;
using AuthSystem.Core.Entities;
using AuthSystem.Core.DTOs;

namespace AuthSystem.Core.Mappings
{
    // ну тут маппинг. превращения всякие
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<UserDTO, UserEntity>().ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
            CreateMap<UserEntity, UserDTO>();
            CreateMap<RegisterDTO, UserEntity>();
        }
    }
}
