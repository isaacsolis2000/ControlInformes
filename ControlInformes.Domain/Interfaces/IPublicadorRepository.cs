using ControlInformes.Domain.Entities;

namespace ControlInformes.Domain.Interfaces;

public interface IPublicadorRepository : IGenericRepository<Publicador>
{
    Task<Publicador?> GetByNombreAsync(string nombreCompleto);
}
