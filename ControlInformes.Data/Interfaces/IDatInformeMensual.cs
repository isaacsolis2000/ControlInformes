using ControlInformes.Domain.Entities;

namespace ControlInformes.Data.Interfaces;

public interface IDatInformeMensual
{
    Task<List<InformeMensual>> GetByMesAsync(int ano, int mes);
    Task<List<InformeMensual>> GetByPublicadorAsync(Guid idPublicador);
    Task<InformeMensual?> GetByPublicadorMesAsync(Guid idPublicador, int ano, int mes);
    Task AddAsync(InformeMensual informe);
    void Update(InformeMensual informe);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
