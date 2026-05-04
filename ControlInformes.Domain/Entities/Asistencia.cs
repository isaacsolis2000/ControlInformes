using ControlInformes.Domain.Enums;
namespace ControlInformes.Domain.Entities;

public class Asistencia
{
    public Guid IdAsistencia { get; set; }
    public DateTime FechaReunion { get; set; }
    public TipoReunion? TipoReunion { get; set; }   // Nullable: puede no haber reunión
    public int CantidadPresencial { get; set; }
    public int CantidadVirtual { get; set; }
    public int Total => CantidadPresencial + CantidadVirtual;  // Calculado
    public string? Observacion { get; set; }
}