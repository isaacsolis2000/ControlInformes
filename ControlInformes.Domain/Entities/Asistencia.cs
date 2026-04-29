using ControlInformes.Domain.Enums;

namespace ControlInformes.Domain.Entities;

public class Asistencia
{
    public Guid IdAsistencia { get; set; }
    public DateTime Fecha { get; set; }
    public TipoReunion TipoReunion { get; set; }
    public int Cantidad { get; set; }
}
