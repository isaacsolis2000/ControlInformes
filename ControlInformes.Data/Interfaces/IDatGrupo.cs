using ControlInformes.Domain.Entities;

namespace ControlInformes.Data.Interfaces;

public interface IDatGrupo
{
    Task<List<Grupo>> GetAllAsync();
    Task<Grupo?> GetByIdAsync(Guid id);
    Task<Grupo?> GetByNombreAsync(string nombre);
    Task<List<Grupo>> GetConMiembrosAsync();       
    Task<Grupo?> GetConCapitanAsync(Guid id);       
    Task AddAsync(Grupo grupo);
    void Update(Grupo grupo);
    void Delete(Grupo grupo);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task<Grupo?> GetByCapitanAsync(Guid idCapitan);
}