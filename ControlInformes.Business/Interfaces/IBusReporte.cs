using ControlInformes.Business.DTOs;
using ControlInformes.Utils;

namespace ControlInformes.Business.Interfaces;

public interface IBusReporte
{
    Task<ApiResponse<ResumenMensualDto>> GetResumenMensualAsync(int ano, int mes);
}
