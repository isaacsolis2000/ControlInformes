namespace ControlInformes.Domain.Entities;

public class Grupo
{
    public Guid IdGrupo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Capitan { get; set; } = string.Empty;

    public ICollection<PublicadorGrupo> PublicadorGrupos { get; set; } = new List<PublicadorGrupo>();
}
