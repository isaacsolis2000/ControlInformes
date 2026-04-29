using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControlInformes.API.Controllers;

[ApiController]
[Route("api/asistencia")]
public class AsistenciaController : ControllerBase
{
    private readonly IBusAsistencia _busAsistencia;

    public AsistenciaController(IBusAsistencia busAsistencia)
    {
        _busAsistencia = busAsistencia;
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarAsistenciaDto dto)
    {
        var response = await _busAsistencia.RegistrarAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetByRango([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {
        var response = await _busAsistencia.GetByRangoAsync(fechaInicio, fechaFin);
        return StatusCode(response.HttpCode, response);
    }
}
