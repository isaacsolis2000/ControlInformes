using ControlInformes.Application.DTOs;
using MediatR;

namespace ControlInformes.Application.Features.Publicadores.Queries;

public record ObtenerPublicadoresQuery : IRequest<IReadOnlyList<PublicadorDto>>;
