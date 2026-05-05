using ControlInformes.Domain.Enums;

namespace ControlInformes.Business.DTOs;

public class PublicadorDto
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
    public Guid? IdGrupo { get; set; }
    public string NombreGrupo { get; set; } = string.Empty;
    public bool Inactivo { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class CrearPublicadorDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    public DateTime? FechaBautismo { get; set; }
    public Genero Genero { get; set; } = Genero.Hombre;
    public CondicionEspiritual CondicionEspiritual { get; set; }
    public TipoPublicador Tipo { get; set; }
    public RolCongregacion Rol { get; set; } = RolCongregacion.Ninguno;
    public Guid? IdGrupo { get; set; }
    public bool Inactivo { get; set; }
}

public class ActualizarPublicadorDto
{
    public Guid IdPublicador { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    public DateTime? FechaBautismo { get; set; }
    public Genero Genero { get; set; }
    public CondicionEspiritual CondicionEspiritual { get; set; }
    public TipoPublicador Tipo { get; set; }
    public RolCongregacion Rol { get; set; }
    public Guid? IdGrupo { get; set; }
    public bool Inactivo { get; set; }
}

