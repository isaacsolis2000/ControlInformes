using MediatR;

namespace ControlInformes.Application.Features.Publicadores.Commands;

public record EliminarPublicadorCommand(Guid IdPublicador) : IRequest<Unit>;
