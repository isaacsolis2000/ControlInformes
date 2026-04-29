using ControlInformes.Domain.Enums;

namespace ControlInformes.Business.DTOs;

public class AsistenciaDto
{
    public Guid IdAsistencia { get; set; }
    public DateTime Fecha { get; set; }
    public TipoReunion TipoReunion { get; set; }
    public int Cantidad { get; set; }
}

public class RegistrarAsistenciaDto
{
    public DateTime Fecha { get; set; }
    public TipoReunion TipoReunion { get; set; }
    public int Cantidad { get; set; }
}
