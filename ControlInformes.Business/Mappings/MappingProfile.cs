using AutoMapper;
using ControlInformes.Business.DTOs;
using ControlInformes.Domain.Entities;

namespace ControlInformes.Business.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Publicador, PublicadorDto>().ReverseMap();
        CreateMap<CrearPublicadorDto, Publicador>();
        CreateMap<ActualizarPublicadorDto, Publicador>();

        CreateMap<InformeMensual, InformeMensualDto>()
            .ForMember(d => d.NombrePublicador, opt => opt.MapFrom(s => s.Publicador != null ? s.Publicador.NombreCompleto : string.Empty));

        CreateMap<Asistencia, AsistenciaDto>().ReverseMap();
        CreateMap<RegistrarAsistenciaDto, Asistencia>();
    }
}
