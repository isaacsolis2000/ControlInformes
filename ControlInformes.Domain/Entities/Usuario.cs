namespace ControlInformes.Domain.Entities;

public class Usuario
{
    public Guid IdUsuario { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
