using ControlInformes.Business.DTOs;
using ControlInformes.Utils;

namespace ControlInformes.Business.Interfaces;

public interface IBusGrupo
{
    Task<ApiResponse<List<GrupoDto>>> GetAllAsync();
    Task<ApiResponse<GrupoDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<GrupoDto>> GetConMiembrosAsync(Guid id);
    Task<ApiResponse<Guid>> CrearAsync(CrearGrupoDto dto);
    Task<ApiResponse<string>> ActualizarAsync(ActualizarGrupoDto dto);
    Task<ApiResponse<string>> EliminarAsync(Guid id);
    Task<ApiResponse<string>> AsignarPublicadoresAsync(AsignarPublicadoresDto dto);
}