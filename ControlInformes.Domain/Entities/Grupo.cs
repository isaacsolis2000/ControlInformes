namespace ControlInformes.Domain.Entities;

public class Grupo
{
    public Guid IdGrupo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    // Capitán del grupo (es un Publicador)
    public Guid IdCapitan { get; set; }
    public Publicador Capitan { get; set; } = null!;
    // Miembros del grupo
    public ICollection<Publicador> Publicadores { get; set; } = new List<Publicador>();

}
