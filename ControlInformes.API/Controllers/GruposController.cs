using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControlInformes.API.Controllers;

[ApiController]
[Route("api/grupos")]
public class GruposController : ControllerBase
{
    private readonly IBusGrupo _busGrupo;

    public GruposController(IBusGrupo busGrupo)
    {
        _busGrupo = busGrupo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _busGrupo.GetAllAsync();
        return StatusCode(response.HttpCode, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _busGrupo.GetByIdAsync(id);
        return StatusCode(response.HttpCode, response);
    }

    //[HttpGet("{id}/miembros")]
    //public async Task<IActionResult> GetConMiembros(Guid id)
    //{
    //    var response = await _busGrupo.GetConMiembrosAsync(id);
    //    return StatusCode(response.HttpCode, response);
    //}

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearGrupoDto dto)
    {
        var response = await _busGrupo.CrearAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ActualizarGrupoDto dto)
    {
        if (id != dto.IdGrupo)
            return BadRequest(new { HasError = true, Mensaje = "El ID no coincide." });

        var response = await _busGrupo.ActualizarAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _busGrupo.EliminarAsync(id);
        return StatusCode(response.HttpCode, response);
    }

    [HttpPost("asignar-publicadores")]
    public async Task<IActionResult> AsignarPublicadores([FromBody] AsignarPublicadoresDto dto)
    {
        var response = await _busGrupo.AsignarPublicadoresAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

}