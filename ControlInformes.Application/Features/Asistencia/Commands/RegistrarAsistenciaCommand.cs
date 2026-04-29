using ControlInformes.Domain.Enums;
using MediatR;

namespace ControlInformes.Application.Features.Asistencia.Commands;

public record RegistrarAsistenciaCommand(
    DateTime Fecha,
    TipoReunion TipoReunion,
    int Cantidad
) : IRequest<Guid>;
