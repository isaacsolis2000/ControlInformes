using ControlInformes.Data.Interfaces;
using ControlInformes.Data.Persistence;
using ControlInformes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlInformes.Data.Implementations;

public class DatGrupo : IDatGrupo
{
    private readonly AppDbContext _context;

    public DatGrupo(AppDbContext context)
    {
        _context = context;
    }

    // Solo grupos sin navegación
    public async Task<List<Grupo>> GetAllAsync()
        => await _context.Grupos.ToListAsync();

    public async Task<Grupo?> GetByIdAsync(Guid id)
        => await _context.Grupos.FindAsync(id);

    public async Task<Grupo?> GetByNombreAsync(string nombre)
        => await _context.Grupos
            .FirstOrDefaultAsync(g => g.Nombre == nombre);

    // Grupo con su capitán incluido
    public async Task<Grupo?> GetConCapitanAsync(Guid id)
        => await _context.Grupos
            .Include(g => g.Capitan)
            .FirstOrDefaultAsync(g => g.IdGrupo == id);

    // Grupos con capitán y lista de miembros
    public async Task<List<Grupo>> GetConMiembrosAsync()
        => await _context.Grupos
            .Include(g => g.Capitan)
            .Include(g => g.Publicadores)
            .ToListAsync();

    public async Task AddAsync(Grupo grupo)
        => await _context.Grupos.AddAsync(grupo);

    public void Update(Grupo grupo)
        => _context.Grupos.Update(grupo);

    public void Delete(Grupo grupo)
        => _context.Grupos.Remove(grupo);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}