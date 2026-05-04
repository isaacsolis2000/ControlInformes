using ControlInformes.Domain.Enums;

namespace ControlInformes.Business.DTOs;

public class PublicadorDto
{
    public Guid IdPublicador { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public DateTime? FechaBautismo { get; set; }
    public TipoPublicador Tipo { get; set; }
    public bool Inactivo { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class CrearPublicadorDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public DateTime? FechaBautismo { get; set; }
    public TipoPublicador Tipo { get; set; }
    public Guid? IdGrupo { get; set; }
    public bool Inactivo { get; set; }
}

public class ActualizarPublicadorDto
{
    public Guid IdPublicador { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    public DateTime? FechaBautismo { get; set; }
    public TipoPublicador Tipo { get; set; }
    public Guid? IdGrupo { get; set; }
}
