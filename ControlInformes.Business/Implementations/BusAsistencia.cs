using AutoMapper;
using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using ControlInformes.Data.Interfaces;
using ControlInformes.Domain.Entities;
using ControlInformes.Utils;
using Microsoft.Extensions.Logging;

namespace ControlInformes.Business.Implementations;

public class BusAsistencia : IBusAsistencia
{
    private readonly IDatAsistencia _datAsistencia;
    private readonly IMapper _mapper;
    private readonly ILogger<BusAsistencia> _logger;

    public BusAsistencia(IDatAsistencia datAsistencia, IMapper mapper, ILogger<BusAsistencia> logger)
    {
        _datAsistencia = datAsistencia;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<AsistenciaDto>>> GetPaginadoAsync(FiltroAsistenciaDto filtro)
    {
        try
        {
            var (items, total) = await _datAsistencia.GetPaginadoAsync(
                filtro.Ano, filtro.Mes, filtro.TipoReunion, filtro.Pagina, filtro.TamanoPagina);

            var dtos = items.Select(a => new AsistenciaDto
            {
                IdAsistencia = a.IdAsistencia,
                FechaReunion = a.FechaReunion,
                TipoReunion = a.TipoReunion,
                TipoReunionDescripcion = a.TipoReunion?.ToString() ?? "Sin reunión",
                CantidadPresencial = a.CantidadPresencial,
                CantidadVirtual = a.CantidadVirtual,
                Total = a.Total,
                Observacion = a.Observacion
            }).ToList();

            var result = new PagedResult<AsistenciaDto>
            {
                Items = dtos,
                TotalRegistros = total,
                Pagina = filtro.Pagina,
                TamanoPagina = filtro.TamanoPagina
            };

            return ApiResponse<PagedResult<AsistenciaDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asistencias paginadas.");
            return ApiResponse<PagedResult<AsistenciaDto>>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<AsistenciaDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var asistencia = await _datAsistencia.GetByIdAsync(id);
            if (asistencia == null)
                return ApiResponse<AsistenciaDto>.NotFound(
                    $"Asistencia con Id ({id}) no encontrada.", ErrorCatalog.EntidadNoEncontrada);

            var result = _mapper.Map<AsistenciaDto>(asistencia);
            return ApiResponse<AsistenciaDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asistencia por Id: {Id}.", id);
            return ApiResponse<AsistenciaDto>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<Guid>> RegistrarAsync(RegistrarAsistenciaDto dto)
    {
        try
        {
            // Validar duplicado si viene con tipo
            if (dto.TipoReunion.HasValue)
            {
                var existente = await _datAsistencia.GetByFechaYTipoAsync(dto.FechaReunion, dto.TipoReunion.Value);
                if (existente != null)
                    return ApiResponse<Guid>.Error(
                        $"Ya existe una reunión {dto.TipoReunion} registrada para esa fecha.",
                        ErrorCatalog.EntidadDuplicada);
            }

            var asistencia = _mapper.Map<Asistencia>(dto);
            asistencia.IdAsistencia = Guid.NewGuid();

            await _datAsistencia.AddAsync(asistencia);
            await _datAsistencia.SaveChangesAsync();

            _logger.LogInformation("Asistencia registrada: {Id}", asistencia.IdAsistencia);
            return ApiResponse<Guid>.Ok(asistencia.IdAsistencia, "Asistencia registrada.", 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar asistencia.");
            return ApiResponse<Guid>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<Guid>> RegistrarFechaAsync(RegistrarFechaDto dto)
    {
        try
        {
            var asistencia = new Asistencia
            {
                IdAsistencia = Guid.NewGuid(),
                FechaReunion = dto.FechaReunion,
                TipoReunion = null,   // Sin reunión
                CantidadPresencial = 0,
                CantidadVirtual = 0,
                Observacion = dto.Observacion
            };

            await _datAsistencia.AddAsync(asistencia);
            await _datAsistencia.SaveChangesAsync();

            _logger.LogInformation("Fecha registrada sin reunión: {Fecha}", dto.FechaReunion);
            return ApiResponse<Guid>.Ok(asistencia.IdAsistencia, "Fecha registrada.", 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar fecha.");
            return ApiResponse<Guid>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<string>> ActualizarAsync(ActualizarAsistenciaDto dto)
    {
        try
        {
            var asistencia = await _datAsistencia.GetByIdAsync(dto.IdAsistencia);
            if (asistencia == null)
                return ApiResponse<string>.NotFound(
                    $"Asistencia con Id ({dto.IdAsistencia}) no encontrada.", ErrorCatalog.EntidadNoEncontrada);

            // Validar duplicado excluyendo el actual
            if (dto.TipoReunion.HasValue)
            {
                var existente = await _datAsistencia.GetByFechaYTipoAsync(dto.FechaReunion, dto.TipoReunion.Value);
                if (existente != null && existente.IdAsistencia != dto.IdAsistencia)
                    return ApiResponse<string>.Error(
                        $"Ya existe una reunión {dto.TipoReunion} registrada para esa fecha.",
                        ErrorCatalog.EntidadDuplicada);
            }

            asistencia.FechaReunion = dto.FechaReunion;
            asistencia.TipoReunion = dto.TipoReunion;
            asistencia.CantidadPresencial = dto.CantidadPresencial;
            asistencia.CantidadVirtual = dto.CantidadVirtual;
            asistencia.Observacion = dto.Observacion;

            _datAsistencia.Update(asistencia);
            await _datAsistencia.SaveChangesAsync();

            _logger.LogInformation("Asistencia actualizada: {Id}", dto.IdAsistencia);
            return ApiResponse<string>.Ok("Actualizado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar asistencia: {Id}.", dto.IdAsistencia);
            return ApiResponse<string>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<string>> EliminarAsync(Guid id)
    {
        try
        {
            var asistencia = await _datAsistencia.GetByIdAsync(id);
            if (asistencia == null)
                return ApiResponse<string>.NotFound(
                    $"Asistencia con Id ({id}) no encontrada.", ErrorCatalog.EntidadNoEncontrada);

            _datAsistencia.Delete(asistencia);
            await _datAsistencia.SaveChangesAsync();

            _logger.LogInformation("Asistencia eliminada: {Id}", id);
            return ApiResponse<string>.Ok("Eliminado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar asistencia: {Id}.", id);
            return ApiResponse<string>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }
}