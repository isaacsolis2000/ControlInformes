namespace ControlInformes.Domain.Entities;

public class PublicadorGrupo
{
    public Guid IdPublicadorGrupo { get; set; }
    public Guid IdPublicador { get; set; }
    public Guid IdGrupo { get; set; }

    public Publicador Publicador { get; set; } = null!;
    public Grupo Grupo { get; set; } = null!;
}
