using ControlInformes.Domain.Enums;
namespace ControlInformes.Business.DTOs;

public class AsistenciaDto
{
    public Guid IdAsistencia { get; set; }
    public DateTime FechaReunion { get; set; }
    public TipoReunion? TipoReunion { get; set; }
    public string TipoReunionDescripcion { get; set; } = string.Empty;
    public int CantidadPresencial { get; set; }
    public int CantidadVirtual { get; set; }
    public int Total { get; set; }
    public string? Observacion { get; set; }
}

public class RegistrarAsistenciaDto
{
    public DateTime FechaReunion { get; set; }
    public TipoReunion? TipoReunion { get; set; }
    public int CantidadPresencial { get; set; }
    public int CantidadVirtual { get; set; }
    public string? Observacion { get; set; }
}

public class ActualizarAsistenciaDto
{
    public Guid IdAsistencia { get; set; }
    public DateTime FechaReunion { get; set; }
    public TipoReunion? TipoReunion { get; set; }
    public int CantidadPresencial { get; set; }
    public int CantidadVirtual { get; set; }
    public string? Observacion { get; set; }
}

// Solo fecha y observacion (bitácora sin reunión)
public class RegistrarFechaDto
{
    public DateTime FechaReunion { get; set; }
    public string? Observacion { get; set; }
}

public class FiltroAsistenciaDto
{
    public int? Ano { get; set; }
    public int? Mes { get; set; }
    public TipoReunion? TipoReunion { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 20;
}

public class TarjetaReunionesDto
{
    public int AnoServicio { get; set; }
}