using ControlInformes.Domain.Enums;

namespace ControlInformes.Business.DTOs;

public class InformeMensualDto
{
    public Guid IdInformeMensual { get; set; }
    public Guid IdPublicador { get; set; }
    public string NombrePublicador { get; set; } = string.Empty;
    public int Ano { get; set; }
    public int Mes { get; set; }
    public bool Participo { get; set; }
    public int CursosBiblicos { get; set; }
    public int? Horas { get; set; }
    public TipoPublicador Tipo { get; set; }
}

public class RegistrarInformeDto
{
    public Guid IdPublicador { get; set; }
    public int Ano { get; set; }
    public int Mes { get; set; }
    public bool Participo { get; set; }
    public int CursosBiblicos { get; set; }
    public int? Horas { get; set; }
}
