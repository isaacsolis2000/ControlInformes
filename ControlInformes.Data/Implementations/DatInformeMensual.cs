using ControlInformes.Data.Interfaces;
using ControlInformes.Data.Persistence;
using ControlInformes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlInformes.Data.Implementations;

public class DatInformeMensual : IDatInformeMensual
{
    private readonly AppDbContext _context;

    public DatInformeMensual(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<InformeMensual>> GetByMesAsync(int ano, int mes)
        => await _context.InformesMensuales
            .Include(i => i.Publicador)
            .Where(i => i.Ano == ano && i.Mes == mes)
            .ToListAsync();

    public async Task<List<InformeMensual>> GetByPublicadorAsync(Guid idPublicador)
        => await _context.InformesMensuales
            .Where(i => i.IdPublicador == idPublicador)
            .OrderBy(i => i.Ano)
            .ThenBy(i => i.Mes)
            .ToListAsync();

    public async Task<InformeMensual?> GetByPublicadorMesAsync(Guid idPublicador, int ano, int mes)
        => await _context.InformesMensuales
            .FirstOrDefaultAsync(i => i.IdPublicador == idPublicador && i.Ano == ano && i.Mes == mes);

    public async Task AddAsync(InformeMensual informe)
        => await _context.InformesMensuales.AddAsync(informe);

    public void Update(InformeMensual informe)
        => _context.InformesMensuales.Update(informe);

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
        => await _context.SaveChangesAsync(cancellationToken);
}
