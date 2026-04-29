using AutoMapper;
using ControlInformes.Application.DTOs;
using ControlInformes.Domain.Entities;

namespace ControlInformes.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Publicador, PublicadorDto>().ReverseMap();

        CreateMap<InformeMensual, InformeMensualDto>()
            .ForMember(d => d.NombrePublicador, opt => opt.MapFrom(s => s.Publicador != null ? s.Publicador.NombreCompleto : string.Empty));

        CreateMap<Asistencia, AsistenciaDto>().ReverseMap();
    }
}
