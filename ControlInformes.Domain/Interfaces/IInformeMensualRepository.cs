using ControlInformes.Domain.Entities;

namespace ControlInformes.Domain.Interfaces;

public interface IInformeMensualRepository : IGenericRepository<InformeMensual>
{
    Task<IReadOnlyList<InformeMensual>> GetByMesAsync(int año, int mes);
    Task<IReadOnlyList<InformeMensual>> GetByPublicadorAsync(Guid idPublicador);
    Task<InformeMensual?> GetByPublicadorMesAsync(Guid idPublicador, int año, int mes);
}
