using AutoMapper;
using ControlInformes.Application.DTOs;
using ControlInformes.Domain.Interfaces;
using MediatR;

namespace ControlInformes.Application.Features.Asistencia.Queries;

public class ObtenerAsistenciaPorRangoQueryHandler : IRequestHandler<ObtenerAsistenciaPorRangoQuery, IReadOnlyList<AsistenciaDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ObtenerAsistenciaPorRangoQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AsistenciaDto>> Handle(ObtenerAsistenciaPorRangoQuery request, CancellationToken cancellationToken)
    {
        var asistencias = await _unitOfWork.Asistencias.GetByRangoAsync(request.FechaInicio, request.FechaFin);
        return _mapper.Map<IReadOnlyList<AsistenciaDto>>(asistencias);
    }
}
