using ControlInformes.Domain.Enums;
using MediatR;

namespace ControlInformes.Application.Features.Publicadores.Commands;

public record CrearPublicadorCommand(
    string NombreCompleto,
    DateTime FechaNacimiento,
    DateTime? FechaBautismo,
    TipoPublicador Tipo
) : IRequest<Guid>;
