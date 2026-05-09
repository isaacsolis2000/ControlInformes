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
    public async Task<IActionResult> DescargarTarjetaReuniones([FromQuery] int anoServicio)
    {
        var response = await _busAsistencia.DescargarTarjetaReunionesAsync(anoServicio);
        if (response.HasError)
            return StatusCode(response.HttpCode, response);

        return File(
            response.Result!,
            "application/pdf",
            $"tarjeta_reuniones_{anoServicio}-{anoServicio + 1}.pdf");
    }
}