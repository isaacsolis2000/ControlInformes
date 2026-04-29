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
}
