using ControlInformes.Business.DTOs;
using ControlInformes.Utils;

namespace ControlInformes.Business.Interfaces;

public interface IBusGrupo
{
    Task<ApiResponse<List<GrupoDto>>> GetAllAsync();
    Task<ApiResponse<GrupoDto>> GetByIdAsync(Guid id);
    // ❌ Eliminar: GetConMiembrosAsync
    Task<ApiResponse<Guid>> CrearAsync(CrearGrupoDto dto);
    Task<ApiResponse<string>> ActualizarAsync(ActualizarGrupoDto dto);
    Task<ApiResponse<string>> EliminarAsync(Guid id);
    Task<ApiResponse<string>> AsignarPublicadoresAsync(AsignarPublicadoresDto dto);
    Task<ApiResponse<List<PublicadorDto>>> GetMiembrosAsync(Guid idGrupo); // ← nuevo
    Task<ApiResponse<string>> QuitarPublicadoresAsync(QuitarPublicadoresDto dto); // ← nuevo
}