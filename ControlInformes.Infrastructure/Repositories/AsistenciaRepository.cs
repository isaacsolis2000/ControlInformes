using ControlInformes.Domain.Entities;
using ControlInformes.Domain.Interfaces;
using ControlInformes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ControlInformes.Infrastructure.Repositories;

public class AsistenciaRepository : GenericRepository<Asistencia>, IAsistenciaRepository
{
    public AsistenciaRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Asistencia>> GetByRangoAsync(DateTime fechaInicio, DateTime fechaFin)
        => await _dbSet.Where(a => a.Fecha >= fechaInicio && a.Fecha <= fechaFin).OrderBy(a => a.Fecha).ToListAsync();
}
