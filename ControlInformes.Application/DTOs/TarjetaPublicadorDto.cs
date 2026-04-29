namespace ControlInformes.Application.DTOs;

public class TarjetaPublicadorDto
{
    public Guid IdPublicador { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public int AnoServicioInicio { get; set; }
    public int AnoServicioFin { get; set; }
    public List<TarjetaMesDto> Meses { get; set; } = new();
}

public class TarjetaMesDto
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public bool Participo { get; set; }
    public int CursosBiblicos { get; set; }
    public int? Horas { get; set; }
    public string? Notas { get; set; }
}
