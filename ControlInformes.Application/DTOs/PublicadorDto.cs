using ControlInformes.Domain.Enums;

namespace ControlInformes.Application.DTOs;

public class PublicadorDto
{
    public Guid IdPublicador { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public DateTime? FechaBautismo { get; set; }
    public TipoPublicador Tipo { get; set; }
    public bool Activo { get; set; }
}
