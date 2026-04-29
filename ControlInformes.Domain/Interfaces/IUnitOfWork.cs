namespace ControlInformes.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPublicadorRepository Publicadores { get; }
    IInformeMensualRepository InformesMensuales { get; }
    IAsistenciaRepository Asistencias { get; }
    IGenericRepository<Domain.Entities.Grupo> Grupos { get; }
    IGenericRepository<Domain.Entities.PublicadorGrupo> PublicadorGrupos { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
