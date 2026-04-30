using ControlInformes.Domain.Entities;

namespace ControlInformes.Data.Interfaces;

public interface IDatUsuario
{
    Task<Usuario?> GetByUsernameAsync(string username);
    Task AddAsync(Usuario usuario);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
