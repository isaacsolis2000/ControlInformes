namespace ControlInformes.Business.DTOs;

public class ImportarTarjetasDto
{
    public Guid? IdGrupo { get; set; }    // null = sin grupo
}

public class ResultadoImportacionTarjetasDto
{
    public int Exitosos { get; set; }
    public int Fallidos { get; set; }
    public List<string> Errores { get; set; } = new();
    public List<string> Creados { get; set; } = new();
    public List<string> Actualizados { get; set; } = new();
}