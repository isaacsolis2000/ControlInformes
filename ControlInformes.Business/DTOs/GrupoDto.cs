namespace ControlInformes.Business.DTOs;

public class GrupoDto
{
    public Guid IdGrupo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public Guid IdCapitan { get; set; }
    public string NombreCapitan { get; set; } = string.Empty;
    public int TotalMiembros { get; set; } // ← nuevo
}

public class CrearGrupoDto
{
    public string Nombre { get; set; } = string.Empty;
    public Guid IdCapitan { get; set; }
}

public class ActualizarGrupoDto
{
    public Guid IdGrupo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public Guid IdCapitan { get; set; }
}

public class QuitarPublicadoresDto
{
    public Guid IdGrupo { get; set; }
    public List<Guid> IdPublicadores { get; set; } = new();
}