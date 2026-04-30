namespace ControlInformes.Business.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiracion { get; set; }
}
