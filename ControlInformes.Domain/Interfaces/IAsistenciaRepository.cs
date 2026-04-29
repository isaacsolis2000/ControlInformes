using ControlInformes.Domain.Entities;

namespace ControlInformes.Domain.Interfaces;

public interface IAsistenciaRepository : IGenericRepository<Asistencia>
{
    Task<IReadOnlyList<Asistencia>> GetByRangoAsync(DateTime fechaInicio, DateTime fechaFin);
}
