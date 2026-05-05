using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var response = await _busGrupo.GetByIdAsync(id);
        return StatusCode(response.HttpCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearGrupoDto dto)
    {
        var response = await _busGrupo.CrearAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ActualizarGrupoDto dto)
    {
        var response = await _busGrupo.ActualizarAsync(dto);
        return StatusCode(response.HttpCode, response);
    }

    [HttpDelete("{id:guid}")]
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

    [HttpGet("{id:guid}/miembros")]
    public async Task<IActionResult> GetMiembros(Guid id)
    {
        var response = await _busGrupo.GetMiembrosAsync(id);
        return StatusCode(response.HttpCode, response);
    }

    [HttpPost("quitar-publicadores")]
    public async Task<IActionResult> QuitarPublicadores([FromBody] QuitarPublicadoresDto dto)
    {
        var response = await _busGrupo.QuitarPublicadoresAsync(dto);
        return StatusCode(response.HttpCode, response);
    }
}