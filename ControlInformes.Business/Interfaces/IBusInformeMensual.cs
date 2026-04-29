using ControlInformes.Business.DTOs;
using ControlInformes.Utils;

namespace ControlInformes.Business.Interfaces;

public interface IBusInformeMensual
{
    Task<ApiResponse<Guid>> RegistrarAsync(RegistrarInformeDto dto);
    Task<ApiResponse<List<InformeMensualDto>>> GetByMesAsync(int ano, int mes);
    Task<ApiResponse<List<InformeMensualDto>>> GetHistorialAsync(Guid idPublicador);
}
