using ControlInformes.Domain.Enums;

namespace ControlInformes.Business.DTOs;

public class TarjetaPublicadorDto
{
    public Guid IdPublicador { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    public DateTime? FechaBautismo { get; set; }
    public Genero Genero { get; set; }
    public string GeneroDescripcion { get; set; } = string.Empty;
    public CondicionEspiritual CondicionEspiritual { get; set; }
    public string CondicionEspiritualDescripcion { get; set; } = string.Empty;
    public TipoPublicador Tipo { get; set; }
    public string TipoDescripcion { get; set; } = string.Empty;
    public RolCongregacion Rol { get; set; }
    public string RolDescripcion { get; set; } = string.Empty;
    public string NombreGrupo { get; set; } = string.Empty;
    public int AnoServicioInicio { get; set; }
    public int AnoServicioFin { get; set; }
    public List<TarjetaMesDto> Meses { get; set; } = new();
}

public class TarjetaMesDto
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public string NombreMes { get; set; } = string.Empty;
    public bool Participo { get; set; }
    public int CursosBiblicos { get; set; }
    public int? Horas { get; set; }
    public bool PrecursorAuxiliar { get; set; }
    public string? Notas { get; set; }
}