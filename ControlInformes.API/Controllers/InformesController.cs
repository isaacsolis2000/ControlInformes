using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInformes.API.Controllers;

[Authorize]
[ApiController]
[Route("api/informes")]
public class InformesController : ControllerBase
{
    private readonly IBusInformeMensual _busInforme;

    public InformesController(IBusInformeMensual busInforme)
    {
        _busInforme = busInforme;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginado([FromQuery] FiltroInformeDto filtro)
    {
        var response = await _busInforme.GetPaginadoAsync(filtro);
        return StatusCode(response.HttpCode, response);
    }

    [HttpGet("total")]
    public async Task<IActionResult> GetTotal([FromQuery] int ano, [FromQuery] int mes)
    {
        var response = await _busInforme.GetTotalAsync(ano, mes);
        return StatusCode(response.HttpCode, response);
    }

    [HttpGet("template/{idGrupo:guid}")]
    public async Task<IActionResult> DescargarTemplate(Guid idGrupo)
    {
        var response = await _busInforme.DescargarTemplateAsync(idGrupo);
        if (response.HasError)
            return StatusCode(response.HttpCode, response);

        return File(
            response.Result!,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "template_informes.xlsx");
    }

    [HttpPost("importar")]
    public async Task<IActionResult> Importar(
        [FromQuery] int ano,
        [FromQuery] int mes,
        [FromQuery] Guid idGrupo,
        IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest(new { HasError = true, Mensaje = "El archivo es requerido." });

        var meta = new ImportarInformesDto { Ano = ano, Mes = mes, IdGrupo = idGrupo };
        using var stream = archivo.OpenReadStream();
        var response = await _busInforme.ImportarExcelAsync(meta, stream);
        return StatusCode(response.HttpCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarInformeDto dto)
    {
        var response = await _busInforme.RegistrarAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarInformeDto dto)
    {
        if (id != dto.IdInformeMensual)
            return BadRequest(new { HasError = true, Mensaje = "El ID no coincide." });

        var response = await _busInforme.ActualizarAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        var response = await _busInforme.EliminarAsync(id);
        return StatusCode(response.HttpCode, response);
    }
}