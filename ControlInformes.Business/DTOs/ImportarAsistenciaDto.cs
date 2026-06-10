// ControlInformes.Business.DTOs/ImportarAsistenciaDto.cs
namespace ControlInformes.Business.DTOs;

public class ImportarAsistenciaDto
{
    public DateTime FechaReunion { get; set; }
    public string? TipoReunion { get; set; }   // "Publica", "EntreSemana" o vacío
    public int CantidadPresencial { get; set; }
    public int CantidadVirtual { get; set; }
    public string? Observacion { get; set; }
}

public class ImportarResultadoDto
{
    public int Insertados { get; set; }
    public int Actualizados { get; set; }
    public int Errores { get; set; }
    public List<string> Detalles { get; set; } = new();
}