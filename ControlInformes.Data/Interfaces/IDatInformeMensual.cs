using ControlInformes.Domain.Entities;
using ControlInformes.Domain.Enums;

namespace ControlInformes.Data.Interfaces;

public interface IDatInformeMensual
{
    Task<(List<InformeMensual> Items, int Total)> GetPaginadoAsync(
        int? ano, int? mes, Guid? idGrupo, Guid? idPublicador,
        TipoPublicador? tipo, bool? inactivo, int pagina, int tamanoPagina);
    Task<InformeMensual?> GetByIdAsync(Guid id);
    Task<InformeMensual?> GetByPublicadorMesAsync(Guid idPublicador, int ano, int mes);
    Task<List<InformeMensual>> GetByMesAsync(int ano, int mes);
    Task<List<InformeMensual>> GetByPublicadorAsync(Guid idPublicador);
    Task<List<InformeMensual>> GetByGrupoMesAsync(Guid idGrupo, int ano, int mes);
    Task AddAsync(InformeMensual informe);
    Task AddRangeAsync(List<InformeMensual> informes);
    void Update(InformeMensual informe);
    void Delete(InformeMensual informe);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}