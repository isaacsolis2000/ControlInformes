using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using ControlInformes.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControlInformes.API.Controllers;

[Authorize]
[ApiController]
[Route("api/publicadores")]
public class PublicadoresController : ControllerBase
{
    private readonly IBusPublicador _busPublicador;

    public PublicadoresController(IBusPublicador busPublicador)
    {
        _busPublicador = busPublicador;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _busPublicador.GetAllAsync();
        return StatusCode(response.HttpCode, response);
    }

    // Rutas fijas ANTES de {id}
    [HttpGet("sin-grupo")]
    public async Task<IActionResult> GetSinGrupo()
    {
        var response = await _busPublicador.GetSinGrupoAsync();
        return StatusCode(response.HttpCode, response);
    }

    [HttpGet("listado")]
    public async Task<IActionResult> GetListado([FromQuery] FiltroPublicadorGrupoDto filtro)
    {
        var response = await _busPublicador.GetListadoPaginadoAsync(filtro);
        return StatusCode(response.HttpCode, response);
    }

    // Rutas con parámetro DESPUÉS
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _busPublicador.GetByIdAsync(id);
        return StatusCode(response.HttpCode, response);
    }

    [HttpGet("{id:guid}/tarjeta")]
    public async Task<IActionResult> GetTarjeta(Guid id, [FromQuery] int? anoServicio)
    {
        var response = await _busPublicador.GetTarjetaAsync(id, anoServicio);
        return StatusCode(response.HttpCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearPublicadorDto dto)
    {
        var response = await _busPublicador.CrearAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ActualizarPublicadorDto dto)
    {
        if (id != dto.IdPublicador)
            return BadRequest(new { HasError = true, Mensaje = "El ID no coincide." });

        var response = await _busPublicador.ActualizarAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _busPublicador.EliminarAsync(id);
        return StatusCode(response.HttpCode, response);
    }

    // PublicadoresController — agregar:
    [HttpGet("{id:guid}/tarjeta/pdf")]
    public async Task<IActionResult> DescargarTarjetaPdf(Guid id, [FromQuery] int? anoServicio)
    {
        var response = await _busPublicador.DescargarTarjetaPdfAsync(id, anoServicio);
        if (response.HasError)
            return StatusCode(response.HttpCode, response);

        return File(
            response.Result!,
            "application/pdf",
            $"tarjeta_{id}.pdf");
    }

    [HttpGet("grupo/{idGrupo:guid}/tarjetas/pdf")]
    public async Task<IActionResult> DescargarTarjetasPorGrupo(Guid idGrupo, [FromQuery] int? anoServicio)
    {
        var response = await _busPublicador.DescargarTarjetasPorGrupoAsync(idGrupo, anoServicio);
        if (response.HasError)
            return StatusCode(response.HttpCode, response);

        return File(
            response.Result!,
            "application/zip",
            $"tarjetas_grupo_{anoServicio}.zip");
    }

    [HttpPost("importar-tarjetas")]
    public async Task<IActionResult> ImportarTarjetas(
    [FromForm] IFormFileCollection archivos,
    [FromQuery] Guid? idGrupo)
    {
        if (archivos == null || archivos.Count == 0)
            return BadRequest(new { HasError = true, Mensaje = "Debe proporcionar al menos un archivo PDF." });

        if (archivos.Any(a => !a.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)))
            return BadRequest(new { HasError = true, Mensaje = "Solo se aceptan archivos PDF." });

        var lista = archivos.ToList();
        var response = await _busPublicador.ImportarTarjetasAsync(lista, idGrupo);
        return StatusCode(response.HttpCode, response);
    }

    [HttpGet("tarjeta-resumen/pdf")]
    public async Task<IActionResult> DescargarTarjetaResumen(
    [FromQuery] int anoServicio,
    [FromQuery] TipoResumenPublicador tipo)
    {
        var response = await _busPublicador.DescargarTarjetaResumenAsync(anoServicio, tipo);
        if (response.HasError)
            return StatusCode(response.HttpCode, response);

        var nombreTipo = tipo switch
        {
            TipoResumenPublicador.Publicador => "Publicadores",
            TipoResumenPublicador.PrecursorAuxiliar => "Precursores_Auxiliares",
            TipoResumenPublicador.PrecursorRegular => "Precursores_Regulares",
            _ => "Publicadores"
        };

        return File(
            response.Result!,
            "application/pdf",
            $"Tarjeta_{nombreTipo}_{anoServicio}.pdf");
    }
}
