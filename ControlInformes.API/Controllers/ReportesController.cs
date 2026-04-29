using ControlInformes.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControlInformes.API.Controllers;

[ApiController]
[Route("api/reportes")]
public class ReportesController : ControllerBase
{
    private readonly IBusReporte _busReporte;

    public ReportesController(IBusReporte busReporte)
    {
        _busReporte = busReporte;
    }

    [HttpGet("resumen-mensual")]
    public async Task<IActionResult> GetResumenMensual([FromQuery] int ano, [FromQuery] int mes)
    {
        var response = await _busReporte.GetResumenMensualAsync(ano, mes);
        return StatusCode(response.HttpCode, response);
    }
}
