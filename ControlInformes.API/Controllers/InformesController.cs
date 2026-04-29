using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControlInformes.API.Controllers;

[ApiController]
[Route("api/informes")]
public class InformesController : ControllerBase
{
    private readonly IBusInformeMensual _busInforme;

    public InformesController(IBusInformeMensual busInforme)
    {
        _busInforme = busInforme;
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarInformeDto dto)
    {
        var response = await _busInforme.RegistrarAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetByMes([FromQuery] int ano, [FromQuery] int mes)
    {
        var response = await _busInforme.GetByMesAsync(ano, mes);
        return StatusCode(response.HttpCode, response);
    }

    [HttpGet("historial/{idPublicador}")]
    public async Task<IActionResult> GetHistorial(Guid idPublicador)
    {
        var response = await _busInforme.GetHistorialAsync(idPublicador);
        return StatusCode(response.HttpCode, response);
    }
}
