using AutoMapper;
using SADVO.Application.Dtos;
using SADVO.Domain.Entities.Administrador;

namespace SADVO.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Usuario, UsuarioDto>()
                .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol.Nombre))
                .ReverseMap();

            CreateMap<Ciudadano, CuidadanoDto>().ReverseMap();
            CreateMap<PuestoElectivo, PuestoElectivoDto>().ReverseMap();
            CreateMap<PartidoPolitico, PartidoPoliticoDto>().ReverseMap();
            CreateMap<Eleccion, EleccionDto>().ReverseMap();
        }
    }
}
