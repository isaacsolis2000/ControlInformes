using ControlInformes.Domain.Entities;

namespace ControlInformes.Business.Interfaces;

public interface ITokenService
{
    (string Token, DateTime Expiracion) GenerateToken(Usuario usuario);
}
