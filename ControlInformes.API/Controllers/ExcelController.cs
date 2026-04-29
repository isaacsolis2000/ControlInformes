using ControlInformes.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControlInformes.API.Controllers;

[ApiController]
[Route("api/excel")]
public class ExcelController : ControllerBase
{
    private readonly IBusExcel _busExcel;

    public ExcelController(IBusExcel busExcel)
    {
        _busExcel = busExcel;
    }

    [HttpPost("importar-informes")]
    public async Task<IActionResult> ImportarInformes(IFormFile archivo, [FromQuery] int ano, [FromQuery] int mes)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest(new { HasError = true, Mensaje = "Debe proporcionar un archivo Excel." });

        using var stream = archivo.OpenReadStream();
        var response = await _busExcel.ImportarAsync(stream, ano, mes);
        return StatusCode(response.HttpCode, response);
    }

    [HttpGet("template")]
    public IActionResult DescargarTemplate()
    {
        var archivo = _busExcel.GenerarTemplate();
        return File(archivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "template_informes.xlsx");
    }
}
