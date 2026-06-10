using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInformes.API.Controllers;

[Authorize]
[ApiController]
[Route("api/asistencia")]
public class AsistenciaController : ControllerBase
{
    private readonly IBusAsistencia _busAsistencia;

    public AsistenciaController(IBusAsistencia busAsistencia)
    {
        _busAsistencia = busAsistencia;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaginado([FromQuery] FiltroAsistenciaDto filtro)
    {
        var response = await _busAsistencia.GetPaginadoAsync(filtro);
        return StatusCode(response.HttpCode, response);
    }

    [HttpPost("fecha")]
    public async Task<IActionResult> RegistrarFecha([FromBody] RegistrarFechaDto dto)
    {
        var response = await _busAsistencia.RegistrarFechaAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _busAsistencia.GetByIdAsync(id);
        return StatusCode(response.HttpCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] RegistrarAsistenciaDto dto)
    {
        var response = await _busAsistencia.RegistrarAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

    [HttpPut]
    public async Task<IActionResult> Actualizar([FromBody] ActualizarAsistenciaDto dto)
    {

        var response = await _busAsistencia.ActualizarAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        var response = await _busAsistencia.EliminarAsync(id);
        return StatusCode(response.HttpCode, response);
    }

    [HttpGet("tarjeta/pdf")]
    public async Task<IActionResult> DescargarTarjetaReuniones(
    [FromQuery] int anoServicio1,
    [FromQuery] int anoServicio2)
    {
        var response = await _busAsistencia.DescargarTarjetaReunionesAsync(anoServicio1, anoServicio2);
        if (response.HasError)
            return StatusCode(response.HttpCode, response);

        return File(
            response.Result!,
            "application/pdf",
            $"tarjeta_reuniones_{anoServicio1}-{anoServicio2}.pdf");
    }

    [HttpGet("plantilla/excel")]
    public async Task<IActionResult> DescargarPlantilla()
    {
        var response = await _busAsistencia.DescargarPlantillaAsync();
        if (response.HasError)
            return StatusCode(response.HttpCode, response);

        return File(
            response.Result!,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "plantilla_asistencia.xlsx");
    }

    [HttpPost("importar/excel")]
    public async Task<IActionResult> ImportarPlantilla(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest("Debe adjuntar un archivo.");

        if (!archivo.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Solo se aceptan archivos .xlsx.");

        using var stream = archivo.OpenReadStream();
        var response = await _busAsistencia.ImportarPlantillaAsync(stream);
        return StatusCode(response.HttpCode, response);
    }
}