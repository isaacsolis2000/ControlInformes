using ControlInformes.Domain.Entities;

namespace ControlInformes.Data.Interfaces;

public interface IDatAsistencia
{
    Task<List<Asistencia>> GetByRangoAsync(DateTime fechaInicio, DateTime fechaFin);
    Task AddAsync(Asistencia asistencia);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
