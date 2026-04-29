using ControlInformes.Application.DTOs;
using MediatR;

namespace ControlInformes.Application.Features.Asistencia.Queries;

public record ObtenerAsistenciaPorRangoQuery(DateTime FechaInicio, DateTime FechaFin) : IRequest<IReadOnlyList<AsistenciaDto>>;
