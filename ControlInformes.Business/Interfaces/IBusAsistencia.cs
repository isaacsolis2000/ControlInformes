using ControlInformes.Business.DTOs;
using ControlInformes.Utils;

namespace ControlInformes.Business.Interfaces;

public interface IBusAsistencia
{
    Task<ApiResponse<Guid>> RegistrarAsync(RegistrarAsistenciaDto dto);
    Task<ApiResponse<List<AsistenciaDto>>> GetByRangoAsync(DateTime fechaInicio, DateTime fechaFin);
}
