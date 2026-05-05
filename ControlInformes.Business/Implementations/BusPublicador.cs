using AutoMapper;
using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using ControlInformes.Data.Interfaces;
using ControlInformes.Domain.Entities;
using ControlInformes.Utils;
using Microsoft.Extensions.Logging;

namespace ControlInformes.Business.Implementations;

public class BusPublicador : IBusPublicador
{
    private readonly IDatPublicador _datPublicador;
    private readonly IDatInformeMensual _datInforme;
    private readonly IMapper _mapper;
    private readonly ILogger<BusPublicador> _logger;

    public BusPublicador(IDatPublicador datPublicador, IDatInformeMensual datInforme, IMapper mapper, ILogger<BusPublicador> logger)
    {
        _datPublicador = datPublicador;
        _datInforme = datInforme;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<List<PublicadorDto>>> GetAllAsync()
    {
        try
        {
            var publicadores = await _datPublicador.GetAllAsync();
            var result = _mapper.Map<List<PublicadorDto>>(publicadores);
            return ApiResponse<List<PublicadorDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener publicadores.");
            return ApiResponse<List<PublicadorDto>>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<PublicadorDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var publicador = await _datPublicador.GetByIdAsync(id);
            if (publicador == null)
                return ApiResponse<PublicadorDto>.NotFound($"Publicador con Id ({id}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            var result = _mapper.Map<PublicadorDto>(publicador);
            return ApiResponse<PublicadorDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener publicador por Id: {Id}.", id);
            return ApiResponse<PublicadorDto>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<Guid>> CrearAsync(CrearPublicadorDto dto)
    {
        try
        {
            var publicador = _mapper.Map<Publicador>(dto);
            publicador.IdPublicador = Guid.NewGuid();
            publicador.Activo = true;
            publicador.FechaCreacion = DateTime.Now;
            await _datPublicador.AddAsync(publicador);
            await _datPublicador.SaveChangesAsync();

            _logger.LogInformation("Publicador creado: {Id} - {Nombre}", publicador.IdPublicador, publicador.NombreCompleto);
            return ApiResponse<Guid>.Ok(publicador.IdPublicador, "Publicador creado.", 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear publicador.");
            return ApiResponse<Guid>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<string>> ActualizarAsync(ActualizarPublicadorDto dto)
    {
        try
        {
            var publicador = await _datPublicador.GetByIdAsync(dto.IdPublicador);
            if (publicador == null)
                return ApiResponse<string>.NotFound($"Publicador con Id ({dto.IdPublicador}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            publicador.NombreCompleto = dto.NombreCompleto;
            publicador.FechaNacimiento = dto.FechaNacimiento;
            publicador.FechaBautismo = dto.FechaBautismo;
            publicador.Tipo = dto.Tipo;
            publicador.IdGrupo = dto.IdGrupo;
            publicador.Activo = true;
            publicador.FechaCreacion = DateTime.Now;
            _datPublicador.Update(publicador);
            await _datPublicador.SaveChangesAsync();

            _logger.LogInformation("Publicador actualizado: {Id}", dto.IdPublicador);
            return ApiResponse<string>.Ok("Actualizado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar publicador: {Id}.", dto.IdPublicador);
            return ApiResponse<string>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<string>> EliminarAsync(Guid id)
    {
        try
        {
            var publicador = await _datPublicador.GetByIdAsync(id);
            if (publicador == null)
                return ApiResponse<string>.NotFound($"Publicador con Id ({id}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            _datPublicador.Delete(publicador);
            await _datPublicador.SaveChangesAsync();

            _logger.LogInformation("Publicador eliminado: {Id}", id);
            return ApiResponse<string>.Ok("Eliminado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar publicador: {Id}.", id);
            return ApiResponse<string>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<TarjetaPublicadorDto>> GetTarjetaAsync(Guid idPublicador, int? anoServicio)
    {
        try
        {
        var publicador = await _datPublicador.GetByIdAsync(idPublicador);
        if (publicador == null)
            return ApiResponse<TarjetaPublicadorDto>.NotFound($"Publicador con Id ({idPublicador}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

        var now = DateTime.Now;
        int anoInicio = anoServicio ?? (now.Month >= 9 ? now.Year : now.Year - 1);
        int anoFin = anoInicio + 1;

        var informes = await _datInforme.GetByPublicadorAsync(idPublicador);
        var meses = new List<TarjetaMesDto>();

        for (int m = 9; m <= 12; m++)
        {
            var inf = informes.FirstOrDefault(i => i.Ano == anoInicio && i.Mes == m);
            meses.Add(MapMes(anoInicio, m, inf));
        }
        for (int m = 1; m <= 8; m++)
        {
            var inf = informes.FirstOrDefault(i => i.Ano == anoFin && i.Mes == m);
            meses.Add(MapMes(anoFin, m, inf));
        }

        var tarjeta = new TarjetaPublicadorDto
        {
            IdPublicador = publicador.IdPublicador,
            NombreCompleto = publicador.NombreCompleto,
            AnoServicioInicio = anoInicio,
            AnoServicioFin = anoFin,
            Meses = meses
        };

        return ApiResponse<TarjetaPublicadorDto>.Ok(tarjeta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tarjeta del publicador: {Id}.", idPublicador);
            return ApiResponse<TarjetaPublicadorDto>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    private static TarjetaMesDto MapMes(int ano, int mes, InformeMensual? informe)
    {
        return new TarjetaMesDto
        {
            Ano = ano,
            Mes = mes,
            Participo = informe?.Participo ?? false,
            CursosBiblicos = informe?.CursosBiblicos ?? 0,
            Horas = informe?.Horas,
            Notas = informe == null ? "Sin informe" : null
        };
    }

    public async Task<ApiResponse<List<PublicadorDto>>> GetSinGrupoAsync()
    {
        try
        {
            var publicadores = await _datPublicador.GetSinGrupoAsync();
            var result = _mapper.Map<List<PublicadorDto>>(publicadores);
            return ApiResponse<List<PublicadorDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener publicadores sin grupo.");
            return ApiResponse<List<PublicadorDto>>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<PagedResult<PublicadorGrupoDto>>> GetListadoPaginadoAsync(FiltroPublicadorGrupoDto filtro)
    {
        try
        {
            var (items, total) = await _datPublicador.GetPaginadoConGrupoAsync(
                filtro.IdGrupo,
                filtro.IdPublicador,
                filtro.NombreCompleto,  // ← nuevo
                filtro.Tipo,
                filtro.Inactivo,        // ← nuevo
                filtro.Pagina,
                filtro.TamanoPagina);

            var dtos = items.Select(p => new PublicadorGrupoDto
            {
                IdPublicador = p.IdPublicador,
                NombrePublicador = p.NombreCompleto,
                Tipo = (int)p.Tipo,
                TipoDescripcion = p.Tipo.ToString(),
                IdGrupo = p.IdGrupo,
                NombreGrupo = p.Grupo?.Nombre ?? string.Empty,
                EsCapitan = p.Grupo?.IdCapitan == p.IdPublicador,
                Inactivo = p.Inactivo          // ← nuevo
            }).ToList();

            var result = new PagedResult<PublicadorGrupoDto>
            {
                Items = dtos,
                TotalRegistros = total,
                Pagina = filtro.Pagina,
                TamanoPagina = filtro.TamanoPagina
            };

            return ApiResponse<PagedResult<PublicadorGrupoDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener listado paginado de publicadores por grupo.");
            return ApiResponse<PagedResult<PublicadorGrupoDto>>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }
}
