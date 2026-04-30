using ControlInformes.Data.Interfaces;
using ControlInformes.Data.Persistence;
using ControlInformes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlInformes.Data.Implementations;

public class DatUsuario : IDatUsuario
{
    private readonly AppDbContext _context;

    public DatUsuario(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByUsernameAsync(string username)
        => await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == username);

    public async Task AddAsync(Usuario usuario)
        => await _context.Usuarios.AddAsync(usuario);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
