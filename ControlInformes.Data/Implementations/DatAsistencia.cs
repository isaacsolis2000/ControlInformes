using ControlInformes.Data.Interfaces;
using ControlInformes.Data.Persistence;
using ControlInformes.Domain.Entities;
using ControlInformes.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ControlInformes.Data.Implementations;

public class DatAsistencia : IDatAsistencia
{
    private readonly AppDbContext _context;

    public DatAsistencia(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Asistencia> Items, int Total)> GetPaginadoAsync(
        int? ano, int? mes, TipoReunion? tipoReunion, int pagina, int tamanoPagina)
    {
        var query = _context.Asistencias.AsQueryable();

        if (ano.HasValue)
            query = query.Where(a => a.FechaReunion.Year == ano.Value);

        if (mes.HasValue)
            query = query.Where(a => a.FechaReunion.Month == mes.Value);

        if (tipoReunion.HasValue)
            query = query.Where(a => a.TipoReunion == tipoReunion.Value);

        query = query.OrderByDescending(a => a.FechaReunion);

        var total = await query.CountAsync();
        var items = await query
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Asistencia?> GetByIdAsync(Guid id)
        => await _context.Asistencias.FindAsync(id);

    // Evitar duplicados: misma fecha y tipo
    public async Task<Asistencia?> GetByFechaYTipoAsync(DateTime fecha, TipoReunion tipoReunion)
        => await _context.Asistencias
            .FirstOrDefaultAsync(a => a.FechaReunion.Date == fecha.Date && a.TipoReunion == tipoReunion);

    public async Task AddAsync(Asistencia asistencia)
        => await _context.Asistencias.AddAsync(asistencia);

    public void Update(Asistencia asistencia)
        => _context.Asistencias.Update(asistencia);

    public void Delete(Asistencia asistencia)
        => _context.Asistencias.Remove(asistencia);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}