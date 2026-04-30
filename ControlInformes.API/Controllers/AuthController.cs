using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInformes.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IBusAuth _busAuth;

    public AuthController(IBusAuth busAuth)
    {
        _busAuth = busAuth;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var response = await _busAuth.LoginAsync(dto);
        return StatusCode(response.HttpCode, response);
    }
}
