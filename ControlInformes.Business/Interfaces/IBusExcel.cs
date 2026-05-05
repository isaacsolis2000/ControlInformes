using ControlInformes.Business.DTOs;
using ControlInformes.Utils;

namespace ControlInformes.Business.Interfaces;

public interface IBusExcel
{
    Task<ApiResponse<ImportacionResultadoDto>> ImportarAsync(Stream archivoStream, int ano, int mes, Guid idGrupo);
    Task<ApiResponse<byte[]>> GenerarTemplateAsync(Guid idGrupo);
    Task<ApiResponse<byte[]>> GenerarListadoPublicadoresAsync();
    Task<ApiResponse<byte[]>> GenerarListadoGruposAsync();
}