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
    public async Task<IActionResult> ImportarInformes(
        IFormFile archivo,
        [FromQuery] int ano,
        [FromQuery] int mes,
        [FromQuery] Guid idGrupo)  // ← nuevo
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest(new { HasError = true, Mensaje = "Debe proporcionar un archivo Excel." });

        using var stream = archivo.OpenReadStream();
        var response = await _busExcel.ImportarAsync(stream, ano, mes, idGrupo);
        return StatusCode(response.HttpCode, response);
    }

    [HttpGet("template/{idGrupo:guid}")]
    public async Task<IActionResult> DescargarTemplate(Guid idGrupo)
    {
        var response = await _busExcel.GenerarTemplateAsync(idGrupo);
        if (response.HasError)
            return StatusCode(response.HttpCode, response);

        return File(response.Result!,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "template_informes.xlsx");
    }

    [HttpGet("listado-publicadores")]
    public async Task<IActionResult> DescargarListadoPublicadores()
    {
        var response = await _busExcel.GenerarListadoPublicadoresAsync();
        if (response.HasError)
            return StatusCode(response.HttpCode, response);

        return File(response.Result!,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "listado_publicadores.xlsx");
    }

    [HttpGet("listado-grupos")]
    public async Task<IActionResult> DescargarListadoGrupos()
    {
        var response = await _busExcel.GenerarListadoGruposAsync();
        if (response.HasError)
            return StatusCode(response.HttpCode, response);

        return File(response.Result!,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "listado_grupos.xlsx");
    }
}