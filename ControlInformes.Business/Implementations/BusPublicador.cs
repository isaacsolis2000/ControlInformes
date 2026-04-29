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
        var publicadores = await _datPublicador.GetAllAsync();
        var result = _mapper.Map<List<PublicadorDto>>(publicadores);
        return ApiResponse<List<PublicadorDto>>.Ok(result);
    }

    public async Task<ApiResponse<PublicadorDto>> GetByIdAsync(Guid id)
    {
        var publicador = await _datPublicador.GetByIdAsync(id);
        if (publicador == null)
            return ApiResponse<PublicadorDto>.NotFound($"Publicador con Id ({id}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

        var result = _mapper.Map<PublicadorDto>(publicador);
        return ApiResponse<PublicadorDto>.Ok(result);
    }

    public async Task<ApiResponse<Guid>> CrearAsync(CrearPublicadorDto dto)
    {
        var publicador = _mapper.Map<Publicador>(dto);
        publicador.IdPublicador = Guid.NewGuid();
        publicador.Activo = true;

        await _datPublicador.AddAsync(publicador);
        await _datPublicador.SaveChangesAsync();

        _logger.LogInformation("Publicador creado: {Id} - {Nombre}", publicador.IdPublicador, publicador.NombreCompleto);
        return ApiResponse<Guid>.Ok(publicador.IdPublicador, "Publicador creado.", 201);
    }

    public async Task<ApiResponse<string>> ActualizarAsync(ActualizarPublicadorDto dto)
    {
        var publicador = await _datPublicador.GetByIdAsync(dto.IdPublicador);
        if (publicador == null)
            return ApiResponse<string>.NotFound($"Publicador con Id ({dto.IdPublicador}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

        publicador.NombreCompleto = dto.NombreCompleto;
        publicador.FechaNacimiento = dto.FechaNacimiento;
        publicador.FechaBautismo = dto.FechaBautismo;
        publicador.Tipo = dto.Tipo;
        publicador.Activo = dto.Activo;

        _datPublicador.Update(publicador);
        await _datPublicador.SaveChangesAsync();

        _logger.LogInformation("Publicador actualizado: {Id}", dto.IdPublicador);
        return ApiResponse<string>.Ok("Actualizado correctamente.");
    }

    public async Task<ApiResponse<string>> EliminarAsync(Guid id)
    {
        var publicador = await _datPublicador.GetByIdAsync(id);
        if (publicador == null)
            return ApiResponse<string>.NotFound($"Publicador con Id ({id}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

        _datPublicador.Delete(publicador);
        await _datPublicador.SaveChangesAsync();

        _logger.LogInformation("Publicador eliminado: {Id}", id);
        return ApiResponse<string>.Ok("Eliminado correctamente.");
    }

    public async Task<ApiResponse<TarjetaPublicadorDto>> GetTarjetaAsync(Guid idPublicador, int? anoServicio)
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
}
