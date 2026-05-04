using ControlInformes.Domain.Enums;

namespace ControlInformes.Domain.Entities;

public class Publicador
{
    public Guid IdPublicador { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime? FechaNacimiento { get; set; }
    public DateTime? FechaBautismo { get; set; }
    public TipoPublicador Tipo { get; set; }
    public Guid? IdGrupo { get; set; }
    public bool Inactivo { get; set; } = false;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public Grupo? Grupo { get; set; }
    public ICollection<InformeMensual> InformesMensuales { get; set; } = new List<InformeMensual>();
}
