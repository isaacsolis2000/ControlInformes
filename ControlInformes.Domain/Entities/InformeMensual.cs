using ControlInformes.Domain.Enums;

namespace ControlInformes.Domain.Entities;

public class InformeMensual
{
    public Guid IdInformeMensual { get; set; }
    public Guid IdPublicador { get; set; }
    public int Ano { get; set; }
    public int Mes { get; set; }
    public bool Participo { get; set; }
    public int CursosBiblicos { get; set; }
    public int? Horas { get; set; }
    public TipoPublicador Tipo { get; set; }  // Tipo informativo del mes
    public bool Inactivo { get; set; }
    public string? Observacion { get; set; }

    public Publicador Publicador { get; set; } = null!;
}