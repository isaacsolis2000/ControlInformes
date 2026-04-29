using ControlInformes.Domain.Entities;
using ControlInformes.Domain.Interfaces;
using ControlInformes.Infrastructure.Persistence;

namespace ControlInformes.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IPublicadorRepository Publicadores { get; }
    public IInformeMensualRepository InformesMensuales { get; }
    public IAsistenciaRepository Asistencias { get; }
    public IGenericRepository<Grupo> Grupos { get; }
    public IGenericRepository<PublicadorGrupo> PublicadorGrupos { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Publicadores = new PublicadorRepository(context);
        InformesMensuales = new InformeMensualRepository(context);
        Asistencias = new AsistenciaRepository(context);
        Grupos = new GenericRepository<Grupo>(context);
        PublicadorGrupos = new GenericRepository<PublicadorGrupo>(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
