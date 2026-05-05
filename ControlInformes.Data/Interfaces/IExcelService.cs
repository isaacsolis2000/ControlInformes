namespace ControlInformes.Data.Interfaces;

public interface IExcelService
{
    List<ExcelInformeRow> LeerInformes(Stream stream);
    byte[] GenerarTemplate(List<string> nombresPublicadores); // ← recibe nombres
}

public class ExcelInformeRow
{
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public bool Participo { get; set; }
    public int? Horas { get; set; }
    public int Cursos { get; set; }
    public bool Inactivo { get; set; }          // ← nuevo
    public string? Observacion { get; set; }    // ← nuevo
}
