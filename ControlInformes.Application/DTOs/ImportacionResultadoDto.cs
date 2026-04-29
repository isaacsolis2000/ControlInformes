namespace ControlInformes.Application.DTOs;

public class ImportacionResultadoDto
{
    public int TotalProcesados { get; set; }
    public int Exitosos { get; set; }
    public int Fallidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
