using ControlInformes.Business.DTOs;
using ControlInformes.Utils;

namespace ControlInformes.Business.Interfaces;

public interface IBusPublicador
{
    Task<ApiResponse<List<PublicadorDto>>> GetAllAsync();
    Task<ApiResponse<PublicadorDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<Guid>> CrearAsync(CrearPublicadorDto dto);
    Task<ApiResponse<string>> ActualizarAsync(ActualizarPublicadorDto dto);
    Task<ApiResponse<string>> EliminarAsync(Guid id);
    Task<ApiResponse<TarjetaPublicadorDto>> GetTarjetaAsync(Guid idPublicador, int? anoServicio);
    Task<ApiResponse<List<PublicadorDto>>> GetSinGrupoAsync();
    Task<ApiResponse<PagedResult<PublicadorGrupoDto>>> GetListadoPaginadoAsync(FiltroPublicadorGrupoDto filtro);
}
