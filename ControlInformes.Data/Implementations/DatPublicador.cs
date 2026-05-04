using ControlInformes.Data.Interfaces;
using ControlInformes.Data.Persistence;
using ControlInformes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlInformes.Data.Implementations;

public class DatPublicador : IDatPublicador
{
    private readonly AppDbContext _context;

    public DatPublicador(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Publicador>> GetAllAsync()
        => await _context.Publicadores.ToListAsync();

    public async Task<Publicador?> GetByIdAsync(Guid id)
        => await _context.Publicadores.FindAsync(id);

    public async Task<Publicador?> GetByNombreAsync(string nombreCompleto)
        => await _context.Publicadores.FirstOrDefaultAsync(p => p.NombreCompleto == nombreCompleto);

    public async Task<List<Publicador>> GetActivosAsync()
        => await _context.Publicadores.Where(p => p.Activo).ToListAsync();

    public async Task AddAsync(Publicador publicador)
        => await _context.Publicadores.AddAsync(publicador);

    public void Update(Publicador publicador)
        => _context.Publicadores.Update(publicador);

    public void Delete(Publicador publicador)
        => _context.Publicadores.Remove(publicador);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task<List<Publicador>> GetByGrupoAsync(Guid idGrupo)
    => await _context.Publicadores.Where(p => p.IdGrupo == idGrupo).ToListAsync();

    public async Task<List<Publicador>> GetSinGrupoAsync()
        => await _context.Publicadores.Where(p => p.IdGrupo == null).ToListAsync();

    public async Task<(List<Publicador> Items, int Total)> GetPaginadoConGrupoAsync(
    Guid? idGrupo,
    Guid? idPublicador,
    int? tipo,
    int pagina,
    int tamanoPagina)
    {
        var query = _context.Publicadores
            .Include(p => p.Grupo)
                .ThenInclude(g => g.Capitan)
            .AsQueryable();

        if (idPublicador.HasValue)
            query = query.Where(p => p.IdPublicador == idPublicador.Value);

        if (idGrupo.HasValue)
            query = query.Where(p => p.IdGrupo == idGrupo.Value);

        if (tipo.HasValue)
            query = query.Where(p => (int)p.Tipo == tipo.Value);

        query = query
            .OrderBy(p => p.IdGrupo == null ? 1 : 0)
            .ThenBy(p => p.Grupo != null ? p.Grupo.Nombre : string.Empty)
            .ThenBy(p => p.NombreCompleto);

        var total = await query.CountAsync();

        var items = await query
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return (items, total);
    }
}
