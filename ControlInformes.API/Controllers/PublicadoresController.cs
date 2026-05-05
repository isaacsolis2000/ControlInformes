using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControlInformes.API.Controllers;

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
}
