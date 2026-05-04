using ControlInformes.Domain.Entities;
using ControlInformes.Domain.Enums;

namespace ControlInformes.Data.Interfaces;

public interface IDatAsistencia
{
    Task<(List<Asistencia> Items, int Total)> GetPaginadoAsync(
        int? ano, int? mes, TipoReunion? tipoReunion, int pagina, int tamanoPagina);
    Task<Asistencia?> GetByIdAsync(Guid id);
    Task<Asistencia?> GetByFechaYTipoAsync(DateTime fecha, TipoReunion tipoReunion);
    Task AddAsync(Asistencia asistencia);
    void Update(Asistencia asistencia);
    void Delete(Asistencia asistencia);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}