using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using ControlInformes.Data.Interfaces;
using ControlInformes.Utils;
using Microsoft.Extensions.Logging;

namespace ControlInformes.Business.Implementations;

public class BusAuth : IBusAuth
{
    private readonly IDatUsuario _datUsuario;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<BusAuth> _logger;

    public BusAuth(IDatUsuario datUsuario, IPasswordService passwordService, ITokenService tokenService, ILogger<BusAuth> logger)
    {
        _datUsuario = datUsuario;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        try
        {
            var usuario = await _datUsuario.GetByUsernameAsync(dto.Username);

            if (usuario == null)
                return ApiResponse<LoginResponseDto>.NotFound("Usuario no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            if (!usuario.Activo)
                return ApiResponse<LoginResponseDto>.Fail("Usuario inactivo.", "AUTH_003", 403);

            if (!_passwordService.Verify(dto.Password, usuario.PasswordHash))
                return ApiResponse<LoginResponseDto>.Fail("Credenciales inválidas.", "AUTH_001", 401);

            var (token, expiracion) = _tokenService.GenerateToken(usuario);

            var response = new LoginResponseDto
            {
                Token = token,
                Expiracion = expiracion
            };

            _logger.LogInformation("Login exitoso para usuario: {Username}", dto.Username);
            return ApiResponse<LoginResponseDto>.Ok(response, "Login exitoso.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login para usuario: {Username}", dto.Username);
            return ApiResponse<LoginResponseDto>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }
}
