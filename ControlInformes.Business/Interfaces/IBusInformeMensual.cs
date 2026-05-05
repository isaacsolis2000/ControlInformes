using ControlInformes.Business.DTOs;
using ControlInformes.Utils;

namespace ControlInformes.Business.Interfaces;

public interface IBusInformeMensual
{
    Task<ApiResponse<PagedResult<InformeMensualDto>>> GetPaginadoAsync(FiltroInformeDto filtro);
    Task<ApiResponse<TotalInformeDto>> GetTotalAsync(int ano, int mes);
    Task<ApiResponse<Guid>> RegistrarAsync(RegistrarInformeDto dto);
    Task<ApiResponse<string>> ActualizarAsync(ActualizarInformeDto dto);
    Task<ApiResponse<string>> EliminarAsync(Guid id);
    Task<ApiResponse<ResultadoImportacionDto>> ImportarExcelAsync(ImportarInformesDto meta, Stream archivo);
    Task<ApiResponse<byte[]>> DescargarTemplateAsync(Guid idGrupo);
}