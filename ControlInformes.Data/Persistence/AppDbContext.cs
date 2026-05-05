using ControlInformes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlInformes.Data.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Publicador> Publicadores => Set<Publicador>();
    public DbSet<Grupo> Grupos => Set<Grupo>();
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
            entity.Property(e => e.Genero).HasConversion<int>();                      // ← nuevo
            entity.Property(e => e.CondicionEspiritual).HasConversion<int>();         // ← nuevo
            entity.Property(e => e.Rol).HasConversion<int>();                         // ← nuevo
            entity.Property(e => e.Inactivo).IsRequired();
            entity.Property(e => e.Activo).IsRequired();
            entity.Property(e => e.FechaCreacion).IsRequired();
            entity.HasOne(e => e.Grupo)
                  .WithMany(g => g.Publicadores)
                  .HasForeignKey(e => e.IdGrupo)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Grupo>(entity =>
        {
            entity.HasKey(e => e.IdGrupo);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Capitan)
                  .WithMany()                        
                  .HasForeignKey(e => e.IdCapitan)
                  .OnDelete(DeleteBehavior.Restrict); 
        });


        modelBuilder.Entity<InformeMensual>(entity =>
        {
            entity.HasKey(e => e.IdInformeMensual);
            entity.Property(e => e.Tipo).HasConversion<int>();
            entity.Property(e => e.Inactivo).IsRequired();
            entity.Property(e => e.Observacion).HasMaxLength(500).IsRequired(false); // ← nuevo
            entity.HasOne(e => e.Publicador).WithMany(p => p.InformesMensuales).HasForeignKey(e => e.IdPublicador);
            entity.HasIndex(e => new { e.IdPublicador, e.Ano, e.Mes }).IsUnique();
        });

        modelBuilder.Entity<Asistencia>(entity =>
        {
            entity.HasKey(e => e.IdAsistencia);
            entity.Property(e => e.FechaReunion).IsRequired();
            entity.Property(e => e.TipoReunion).HasConversion<int?>().IsRequired(false); // Nullable
            entity.Property(e => e.CantidadPresencial).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.CantidadVirtual).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.Observacion).HasMaxLength(500).IsRequired(false);

            // Evitar duplicados: misma fecha y tipo
            entity.HasIndex(e => new { e.FechaReunion, e.TipoReunion }).IsUnique();
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
