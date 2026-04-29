using ControlInformes.Application.DTOs;
using MediatR;

namespace ControlInformes.Application.Features.Reportes.Queries;

public record ObtenerResumenMensualQuery(int Ano, int Mes) : IRequest<ResumenMensualDto>;
