using AutoMapper;
using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using ControlInformes.Data.Interfaces;
using ControlInformes.Domain.Entities;
using ControlInformes.Utils;
using Microsoft.Extensions.Logging;

namespace ControlInformes.Business.Implementations;

public class BusGrupo : IBusGrupo
{
    private readonly IDatGrupo _datGrupo;
    private readonly IDatPublicador _datPublicador;
    private readonly IMapper _mapper;
    private readonly ILogger<BusGrupo> _logger;

    public BusGrupo(IDatGrupo datGrupo, IDatPublicador datPublicador, IMapper mapper, ILogger<BusGrupo> logger)
    {
        _datGrupo = datGrupo;
        _datPublicador = datPublicador;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<List<GrupoDto>>> GetAllAsync()
    {
        try
        {
            var grupos = await _datGrupo.GetAllAsync();
            var result = _mapper.Map<List<GrupoDto>>(grupos);
            return ApiResponse<List<GrupoDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener grupos.");
            return ApiResponse<List<GrupoDto>>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<GrupoDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var grupo = await _datGrupo.GetConCapitanAsync(id);
            if (grupo == null)
                return ApiResponse<GrupoDto>.NotFound($"Grupo con Id ({id}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            var result = _mapper.Map<GrupoDto>(grupo);
            return ApiResponse<GrupoDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener grupo por Id: {Id}.", id);
            return ApiResponse<GrupoDto>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<GrupoDto>> GetConMiembrosAsync(Guid id)
    {
        try
        {
            var grupo = await _datGrupo.GetConMiembrosAsync();
            var encontrado = grupo.FirstOrDefault(g => g.IdGrupo == id);
            if (encontrado == null)
                return ApiResponse<GrupoDto>.NotFound($"Grupo con Id ({id}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            var result = _mapper.Map<GrupoDto>(encontrado);
            return ApiResponse<GrupoDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener grupo con miembros: {Id}.", id);
            return ApiResponse<GrupoDto>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<Guid>> CrearAsync(CrearGrupoDto dto)
    {
        try
        {
            // Validar nombre duplicado
            var existente = await _datGrupo.GetByNombreAsync(dto.Nombre);
            if (existente != null)
                return ApiResponse<Guid>.Error($"Ya existe un grupo con el nombre '{dto.Nombre}'.", "00");

            // Validar que el capitán exista
            var capitan = await _datPublicador.GetByIdAsync(dto.IdCapitan);
            if (capitan == null)
                return ApiResponse<Guid>.NotFound($"Publicador capitán con Id ({dto.IdCapitan}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            var grupo = _mapper.Map<Grupo>(dto);
            grupo.IdGrupo = Guid.NewGuid();

            await _datGrupo.AddAsync(grupo);
            await _datGrupo.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("Grupo creado: {Id} - {Nombre}", grupo.IdGrupo, grupo.Nombre);
            return ApiResponse<Guid>.Ok(grupo.IdGrupo, "Grupo creado.", 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear grupo.");
            return ApiResponse<Guid>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<string>> ActualizarAsync(ActualizarGrupoDto dto)
    {
        try
        {
            var grupo = await _datGrupo.GetByIdAsync(dto.IdGrupo);
            if (grupo == null)
                return ApiResponse<string>.NotFound($"Grupo con Id ({dto.IdGrupo}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            // Validar nombre duplicado (ignorar si es el mismo grupo)
            var existente = await _datGrupo.GetByNombreAsync(dto.Nombre);
            if (existente != null && existente.IdGrupo != dto.IdGrupo)
                return ApiResponse<string>.Error($"Ya existe un grupo con el nombre '{dto.Nombre}'.", "00");

            // Validar que el nuevo capitán exista
            var capitan = await _datPublicador.GetByIdAsync(dto.IdCapitan);
            if (capitan == null)
                return ApiResponse<string>.NotFound($"Publicador capitán con Id ({dto.IdCapitan}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            grupo.Nombre = dto.Nombre;
            grupo.IdCapitan = dto.IdCapitan;

            _datGrupo.Update(grupo);
            await _datGrupo.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("Grupo actualizado: {Id}", dto.IdGrupo);
            return ApiResponse<string>.Ok("Actualizado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar grupo: {Id}.", dto.IdGrupo);
            return ApiResponse<string>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<string>> EliminarAsync(Guid id)
    {
        try
        {
            var grupo = await _datGrupo.GetByIdAsync(id);
            if (grupo == null)
                return ApiResponse<string>.NotFound($"Grupo con Id ({id}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            _datGrupo.Delete(grupo);
            await _datGrupo.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("Grupo eliminado: {Id}", id);
            return ApiResponse<string>.Ok("Eliminado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar grupo: {Id}.", id);
            return ApiResponse<string>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<string>> AsignarPublicadoresAsync(AsignarPublicadoresDto dto)
    {
        try
        {
            var grupo = await _datGrupo.GetByIdAsync(dto.IdGrupo);
            if (grupo == null)
                return ApiResponse<string>.NotFound($"Grupo con Id ({dto.IdGrupo}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            foreach (var idPublicador in dto.IdPublicadores)
            {
                var publicador = await _datPublicador.GetByIdAsync(idPublicador);
                if (publicador == null)
                    return ApiResponse<string>.NotFound($"Publicador con Id ({idPublicador}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

                // Validar que no esté en otro grupo
                if (publicador.IdGrupo != null && publicador.IdGrupo != dto.IdGrupo)
                    return ApiResponse<string>.Error(
                        $"El publicador '{publicador.NombreCompleto}' ya pertenece a otro grupo.",
                        ErrorCatalog.EntidadDuplicada);

                publicador.IdGrupo = dto.IdGrupo;
                _datPublicador.Update(publicador);
            }

            await _datPublicador.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("Publicadores asignados al grupo: {Id}", dto.IdGrupo);
            return ApiResponse<string>.Ok("Publicadores asignados correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar publicadores al grupo: {Id}.", dto.IdGrupo);
            return ApiResponse<string>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

}