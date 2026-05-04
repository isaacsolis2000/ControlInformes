using AutoMapper;
using ControlInformes.Business.DTOs;
using ControlInformes.Domain.Entities;

namespace ControlInformes.Business.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Publicador
        CreateMap<Publicador, PublicadorDto>().ReverseMap();
        CreateMap<CrearPublicadorDto, Publicador>();
        CreateMap<ActualizarPublicadorDto, Publicador>();

        // Informe
        CreateMap<InformeMensual, InformeMensualDto>()
            .ForMember(d => d.NombrePublicador, opt => opt.MapFrom(s => s.Publicador != null ? s.Publicador.NombreCompleto : string.Empty));

        // Asistencia
        CreateMap<Asistencia, AsistenciaDto>()
        .ForMember(dest => dest.TipoReunionDescripcion,
            opt => opt.MapFrom(src => src.TipoReunion.HasValue ? src.TipoReunion.ToString() : "Sin reunión"))
        .ForMember(dest => dest.Total,
            opt => opt.MapFrom(src => src.CantidadPresencial + src.CantidadVirtual));

        CreateMap<RegistrarAsistenciaDto, Asistencia>()
            .ForMember(dest => dest.IdAsistencia, opt => opt.Ignore());

        CreateMap<ActualizarAsistenciaDto, Asistencia>()
            .ForMember(dest => dest.IdAsistencia, opt => opt.Ignore());

        // Grupo
        CreateMap<CrearGrupoDto, Grupo>()
            .ForMember(dest => dest.IdGrupo, opt => opt.Ignore())
            .ForMember(dest => dest.Capitan, opt => opt.Ignore())
            .ForMember(dest => dest.Publicadores, opt => opt.Ignore());

        CreateMap<ActualizarGrupoDto, Grupo>()
            .ForMember(dest => dest.Capitan, opt => opt.Ignore())
            .ForMember(dest => dest.Publicadores, opt => opt.Ignore());

        CreateMap<Grupo, GrupoDto>()
            .ForMember(dest => dest.NombreCapitan,
                opt => opt.MapFrom(src => src.Capitan != null ? src.Capitan.NombreCompleto : string.Empty));

        CreateMap<Publicador, PublicadorListadoDto>()
            .ForMember(dest => dest.Tipo,
                opt => opt.MapFrom(src => src.Tipo.ToString()))
            .ForMember(dest => dest.NombreGrupo,
                opt => opt.MapFrom(src => src.Grupo != null ? src.Grupo.Nombre : string.Empty));
    }
}