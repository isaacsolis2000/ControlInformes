using ControlInformes.Business.DTOs;
using ControlInformes.Utils;

namespace ControlInformes.Business.Interfaces;

public interface IBusAsistencia
{
    Task<ApiResponse<PagedResult<AsistenciaDto>>> GetPaginadoAsync(FiltroAsistenciaDto filtro);
    Task<ApiResponse<AsistenciaDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<Guid>> RegistrarAsync(RegistrarAsistenciaDto dto);
    Task<ApiResponse<Guid>> RegistrarFechaAsync(RegistrarFechaDto dto);
    Task<ApiResponse<string>> ActualizarAsync(ActualizarAsistenciaDto dto);
    Task<ApiResponse<string>> EliminarAsync(Guid id);
    Task<ApiResponse<byte[]>> DescargarTarjetaReunionesAsync(int anoServicio1, int anoServicio2);
    Task<ApiResponse<byte[]>> DescargarPlantillaAsync();
    Task<ApiResponse<ImportarResultadoDto>> ImportarPlantillaAsync(Stream archivo);
}