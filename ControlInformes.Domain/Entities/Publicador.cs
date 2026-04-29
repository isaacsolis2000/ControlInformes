using ControlInformes.Domain.Enums;

namespace ControlInformes.Domain.Entities;

public class Publicador
{
    public Guid IdPublicador { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public DateTime? FechaBautismo { get; set; }
    public TipoPublicador Tipo { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<PublicadorGrupo> PublicadorGrupos { get; set; } = new List<PublicadorGrupo>();
    public ICollection<InformeMensual> InformesMensuales { get; set; } = new List<InformeMensual>();
}
