using ControlInformes.Domain.Entities;

namespace ControlInformes.Data.Interfaces;

public interface IDatPublicador
{
    Task<List<Publicador>> GetAllAsync();
    Task<Publicador?> GetByIdAsync(Guid id);
    Task<Publicador?> GetByNombreAsync(string nombreCompleto);
    Task<List<Publicador>> GetActivosAsync();
    Task AddAsync(Publicador publicador);
    void Update(Publicador publicador);
    void Delete(Publicador publicador);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<List<Publicador>> GetByGrupoAsync(Guid idGrupo);
    Task<List<Publicador>> GetSinGrupoAsync();
    Task<(List<Publicador> Items, int Total)> GetPaginadoConGrupoAsync(
     Guid? idGrupo,
     Guid? idPublicador,
     string? nombreCompleto,  // ← nuevo
     int? tipo,
     bool? inactivo,           // ← nuevo
     int pagina,
     int tamanoPagina);
}
