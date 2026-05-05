using ControlInformes.Data.Interfaces;
using ControlInformes.Data.Persistence;
using ControlInformes.Domain.Entities;
using ControlInformes.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ControlInformes.Data.Implementations;

public class DatInformeMensual : IDatInformeMensual
{
    private readonly AppDbContext _context;

    public DatInformeMensual(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<InformeMensual> Items, int Total)> GetPaginadoAsync(
        int? ano, int? mes, Guid? idGrupo, Guid? idPublicador,
        TipoPublicador? tipo, bool? inactivo, int pagina, int tamanoPagina)
    {
        var query = _context.InformesMensuales
            .Include(i => i.Publicador)
                .ThenInclude(p => p.Grupo)
            .AsQueryable();

        if (ano.HasValue)
            query = query.Where(i => i.Ano == ano.Value);

        if (mes.HasValue)
            query = query.Where(i => i.Mes == mes.Value);

        if (idGrupo.HasValue)
            query = query.Where(i => i.Publicador.IdGrupo == idGrupo.Value);

        if (idPublicador.HasValue)
            query = query.Where(i => i.IdPublicador == idPublicador.Value);

        if (tipo.HasValue)
            query = query.Where(i => i.Tipo == tipo.Value);

        if (inactivo.HasValue)
            query = query.Where(i => i.Inactivo == inactivo.Value);

        query = query
            .OrderBy(i => i.Ano)
            .ThenBy(i => i.Mes)
            .ThenBy(i => i.Publicador.Grupo != null ? i.Publicador.Grupo.Nombre : string.Empty)
            .ThenBy(i => i.Tipo)
            .ThenBy(i => i.Publicador.NombreCompleto);

        var total = await query.CountAsync();
        var items = await query
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return (items, total);
    }

    public async Task<InformeMensual?> GetByIdAsync(Guid id)
        => await _context.InformesMensuales
            .Include(i => i.Publicador)
            .FirstOrDefaultAsync(i => i.IdInformeMensual == id);

    public async Task<InformeMensual?> GetByPublicadorMesAsync(Guid idPublicador, int ano, int mes)
        => await _context.InformesMensuales
            .FirstOrDefaultAsync(i => i.IdPublicador == idPublicador && i.Ano == ano && i.Mes == mes);

    public async Task<List<InformeMensual>> GetByMesAsync(int ano, int mes)
        => await _context.InformesMensuales
            .Include(i => i.Publicador)
                .ThenInclude(p => p.Grupo)
            .Where(i => i.Ano == ano && i.Mes == mes)
            .ToListAsync();

    public async Task<List<InformeMensual>> GetByPublicadorAsync(Guid idPublicador)
        => await _context.InformesMensuales
            .Where(i => i.IdPublicador == idPublicador)
            .OrderBy(i => i.Ano)
            .ThenBy(i => i.Mes)
            .ToListAsync();

    public async Task<List<InformeMensual>> GetByGrupoMesAsync(Guid idGrupo, int ano, int mes)
        => await _context.InformesMensuales
            .Include(i => i.Publicador)
            .Where(i => i.Publicador.IdGrupo == idGrupo && i.Ano == ano && i.Mes == mes)
            .ToListAsync();

    public async Task AddAsync(InformeMensual informe)
        => await _context.InformesMensuales.AddAsync(informe);

    public async Task AddRangeAsync(List<InformeMensual> informes)
        => await _context.InformesMensuales.AddRangeAsync(informes);

    public void Update(InformeMensual informe)
        => _context.InformesMensuales.Update(informe);

    public void Delete(InformeMensual informe)
        => _context.InformesMensuales.Remove(informe);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}