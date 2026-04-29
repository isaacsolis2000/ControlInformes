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
}
