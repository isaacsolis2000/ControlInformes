using ControlInformes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlInformes.Data.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Publicador> Publicadores => Set<Publicador>();
    public DbSet<Grupo> Grupos => Set<Grupo>();
    public DbSet<PublicadorGrupo> PublicadorGrupos => Set<PublicadorGrupo>();
    public DbSet<InformeMensual> InformesMensuales => Set<InformeMensual>();
    public DbSet<Asistencia> Asistencias => Set<Asistencia>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Publicador>(entity =>
        {
            entity.HasKey(e => e.IdPublicador);
            entity.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Tipo).HasConversion<int>();
        });

        modelBuilder.Entity<Grupo>(entity =>
        {
            entity.HasKey(e => e.IdGrupo);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Capitan).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<PublicadorGrupo>(entity =>
        {
            entity.HasKey(e => e.IdPublicadorGrupo);
            entity.HasOne(e => e.Publicador).WithMany(p => p.PublicadorGrupos).HasForeignKey(e => e.IdPublicador);
            entity.HasOne(e => e.Grupo).WithMany(g => g.PublicadorGrupos).HasForeignKey(e => e.IdGrupo);
        });

        modelBuilder.Entity<InformeMensual>(entity =>
        {
            entity.HasKey(e => e.IdInformeMensual);
            entity.Property(e => e.Tipo).HasConversion<int>();
            entity.HasOne(e => e.Publicador).WithMany(p => p.InformesMensuales).HasForeignKey(e => e.IdPublicador);
            entity.HasIndex(e => new { e.IdPublicador, e.Ano, e.Mes }).IsUnique();
        });

        modelBuilder.Entity<Asistencia>(entity =>
        {
            entity.HasKey(e => e.IdAsistencia);
            entity.Property(e => e.TipoReunion).HasConversion<int>();
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
        });
    }
}
