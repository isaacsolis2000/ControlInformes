using ControlInformes.Application.DTOs;
using MediatR;

namespace ControlInformes.Application.Features.Informes.Queries;

public record ObtenerHistorialPublicadorQuery(Guid IdPublicador) : IRequest<IReadOnlyList<InformeMensualDto>>;
