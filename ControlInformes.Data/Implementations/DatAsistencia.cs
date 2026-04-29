using ControlInformes.Data.Interfaces;
using ControlInformes.Data.Persistence;
using ControlInformes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlInformes.Data.Implementations;

public class DatAsistencia : IDatAsistencia
{
    private readonly AppDbContext _context;

    public DatAsistencia(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Asistencia>> GetByRangoAsync(DateTime fechaInicio, DateTime fechaFin)
        => await _context.Asistencias
            .Where(a => a.Fecha >= fechaInicio && a.Fecha <= fechaFin)
            .OrderBy(a => a.Fecha)
            .ToListAsync();

    public async Task AddAsync(Asistencia asistencia)
        => await _context.Asistencias.AddAsync(asistencia);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}
