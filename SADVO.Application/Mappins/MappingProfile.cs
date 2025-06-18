using AutoMapper;
using SADVO.Application.Dtos;
using SADVO.Domain.Entities.Administrador;
using SADVO.Domain.Entities.Dirigente;

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
            CreateMap<Candidato, CandidatoDto>().ReverseMap();

            CreateMap<AlianzaPolitica, AlianzaPoliticaDto>()
                .ForMember(dest => dest.PartidoSolicitanteNombre, opt => opt.MapFrom(src => src.PartidoSolicitante.Nombre))
                .ForMember(dest => dest.PartidoSolicitanteSiglas, opt => opt.MapFrom(src => src.PartidoSolicitante.Siglas))
                .ForMember(dest => dest.PartidoReceptorNombre, opt => opt.MapFrom(src => src.PartidoReceptor.Nombre))
                .ForMember(dest => dest.PartidoReceptorSiglas, opt => opt.MapFrom(src => src.PartidoReceptor.Siglas))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()));

        }
    }
}
