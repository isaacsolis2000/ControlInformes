using AutoMapper;
using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using ControlInformes.Data.Interfaces;
using ControlInformes.Domain.Entities;
using ControlInformes.Domain.Enums;
using ControlInformes.Utils;
using Microsoft.Extensions.Logging;

namespace ControlInformes.Business.Implementations;

public class BusInformeMensual : IBusInformeMensual
{
    private readonly IDatInformeMensual _datInforme;
    private readonly IDatPublicador _datPublicador;
    private readonly IMapper _mapper;
    private readonly ILogger<BusInformeMensual> _logger;

    public BusInformeMensual(IDatInformeMensual datInforme, IDatPublicador datPublicador, IMapper mapper, ILogger<BusInformeMensual> logger)
    {
        _datInforme = datInforme;
        _datPublicador = datPublicador;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<Guid>> RegistrarAsync(RegistrarInformeDto dto)
    {
        // Validaciones
        var errores = new List<string>();
        if (dto.Ano < 2000 || dto.Ano > 2100)
            errores.Add("El año debe estar entre 2000 y 2100.");
        if (dto.Mes < 1 || dto.Mes > 12)
            errores.Add("El mes debe estar entre 1 y 12.");
        if (dto.CursosBiblicos < 0)
            errores.Add("Los cursos bíblicos no pueden ser negativos.");
        if (dto.Horas.HasValue && dto.Horas < 0)
            errores.Add("Las horas no pueden ser negativas.");

        if (errores.Count > 0)
            return ApiResponse<Guid>.Fail("Errores de validación.", ErrorCatalog.ValidacionFallida, 400, errores);

        if (!dto.Participo)
        {
            dto.CursosBiblicos = 0;
            dto.Horas = null;
        }

        var publicador = await _datPublicador.GetByIdAsync(dto.IdPublicador);
        if (publicador == null)
            return ApiResponse<Guid>.NotFound($"Publicador con Id ({dto.IdPublicador}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

        int? horas = (publicador.Tipo == TipoPublicador.PrecursorAuxiliar || publicador.Tipo == TipoPublicador.PrecursorRegular)
            ? dto.Horas : null;

        var existente = await _datInforme.GetByPublicadorMesAsync(dto.IdPublicador, dto.Ano, dto.Mes);

        if (existente != null)
        {
            existente.Participo = dto.Participo;
            existente.CursosBiblicos = dto.CursosBiblicos;
            existente.Horas = horas;
            existente.Tipo = publicador.Tipo;
            _datInforme.Update(existente);
            await _datInforme.SaveChangesAsync();

            _logger.LogInformation("Informe actualizado: {Id}", existente.IdInformeMensual);
            return ApiResponse<Guid>.Ok(existente.IdInformeMensual, "Informe actualizado.");
        }

        var informe = new InformeMensual
        {
            IdInformeMensual = Guid.NewGuid(),
            IdPublicador = dto.IdPublicador,
            Ano = dto.Ano,
            Mes = dto.Mes,
            Participo = dto.Participo,
            CursosBiblicos = dto.CursosBiblicos,
            Horas = horas,
            Tipo = publicador.Tipo
        };

        await _datInforme.AddAsync(informe);
        await _datInforme.SaveChangesAsync();

        _logger.LogInformation("Informe registrado: {Id}", informe.IdInformeMensual);
        return ApiResponse<Guid>.Ok(informe.IdInformeMensual, "Informe registrado.", 201);
    }

    public async Task<ApiResponse<List<InformeMensualDto>>> GetByMesAsync(int ano, int mes)
    {
        var informes = await _datInforme.GetByMesAsync(ano, mes);
        var result = _mapper.Map<List<InformeMensualDto>>(informes);
        return ApiResponse<List<InformeMensualDto>>.Ok(result);
    }

    public async Task<ApiResponse<List<InformeMensualDto>>> GetHistorialAsync(Guid idPublicador)
    {
        var informes = await _datInforme.GetByPublicadorAsync(idPublicador);
        var result = _mapper.Map<List<InformeMensualDto>>(informes);
        return ApiResponse<List<InformeMensualDto>>.Ok(result);
    }
}
