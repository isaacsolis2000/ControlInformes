using ControlInformes.Domain.Entities;
using ControlInformes.Domain.Interfaces;
using ControlInformes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ControlInformes.Infrastructure.Repositories;

public class PublicadorRepository : GenericRepository<Publicador>, IPublicadorRepository
{
    public PublicadorRepository(AppDbContext context) : base(context) { }

    public async Task<Publicador?> GetByNombreAsync(string nombreCompleto)
        => await _dbSet.FirstOrDefaultAsync(p => p.NombreCompleto == nombreCompleto);
}
