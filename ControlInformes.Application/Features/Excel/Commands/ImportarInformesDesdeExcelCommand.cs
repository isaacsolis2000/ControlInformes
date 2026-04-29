using ControlInformes.Application.DTOs;
using MediatR;

namespace ControlInformes.Application.Features.Excel.Commands;

public record ImportarInformesDesdeExcelCommand(
    Stream ArchivoStream,
    int Ano,
    int Mes
) : IRequest<ImportacionResultadoDto>;
