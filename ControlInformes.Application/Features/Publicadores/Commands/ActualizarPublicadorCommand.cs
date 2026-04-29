using ControlInformes.Domain.Enums;
using MediatR;

namespace ControlInformes.Application.Features.Publicadores.Commands;

public record ActualizarPublicadorCommand(
    Guid IdPublicador,
    string NombreCompleto,
    DateTime FechaNacimiento,
    DateTime? FechaBautismo,
    TipoPublicador Tipo,
    bool Activo
) : IRequest<Unit>;
