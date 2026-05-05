using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using ControlInformes.Data.Interfaces;
using ControlInformes.Domain.Enums;
using ControlInformes.Utils;
using Microsoft.Extensions.Logging;

namespace ControlInformes.Business.Implementations;

public class BusDashboard : IBusDashboard
{
    private readonly IDatPublicador _datPublicador;
    private readonly IDatInformeMensual _datInforme;
    private readonly IDatAsistencia _datAsistencia;
    private readonly ILogger<BusDashboard> _logger;

    private static readonly string[] _nombresMeses =
    [
        "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
    ];

    public BusDashboard(
        IDatPublicador datPublicador,
        IDatInformeMensual datInforme,
        IDatAsistencia datAsistencia,
        ILogger<BusDashboard> logger)
    {
        _datPublicador = datPublicador;
        _datInforme = datInforme;
        _datAsistencia = datAsistencia;
        _logger = logger;
    }

    public async Task<ApiResponse<DashboardDto>> GetDashboardAsync(int ano, int mes)
    {
        try
        {
            if (ano < 2000 || ano > 2100 || mes < 1 || mes > 12)
                return ApiResponse<DashboardDto>.Fail("Año o mes inválido.", ErrorCatalog.ValidacionFallida, 400);

            var primerDia = new DateTime(ano, mes, 1);

            // ── Datos mes actual ──────────────────────────────────────────────
            var publicadoresActivos = await _datPublicador.GetActivosAsync();
            var informesMes = await _datInforme.GetByMesAsync(ano, mes);
            var (reunionesMes, _) = await _datAsistencia.GetPaginadoAsync(ano, mes, null, 1, 100);

            // ── Datos mes anterior ────────────────────────────────────────────
            var fechaAnterior = primerDia.AddMonths(-1);
            var informesAnterior = await _datInforme.GetByMesAsync(fechaAnterior.Year, fechaAnterior.Month);
            var (reunionesAnterior, _) = await _datAsistencia.GetPaginadoAsync(
                fechaAnterior.Year, fechaAnterior.Month, null, 1, 100);

            // ── Cards publicadores ────────────────────────────────────────────
            var infPublicadores = informesMes.Where(i =>
                i.Tipo == TipoPublicador.Publicador || i.Tipo == TipoPublicador.NoBautizado).ToList();
            var infAuxiliares = informesMes.Where(i => i.Tipo == TipoPublicador.PrecursorAuxiliar).ToList();
            var infRegulares = informesMes.Where(i => i.Tipo == TipoPublicador.PrecursorRegular).ToList();

            var infPubAnterior = informesAnterior.Where(i =>
                i.Tipo == TipoPublicador.Publicador || i.Tipo == TipoPublicador.NoBautizado).ToList();
            var infAuxAnterior = informesAnterior.Where(i => i.Tipo == TipoPublicador.PrecursorAuxiliar).ToList();
            var infRegAnterior = informesAnterior.Where(i => i.Tipo == TipoPublicador.PrecursorRegular).ToList();

            // ── Cards reuniones ───────────────────────────────────────────────
            var reunionesPublicas = reunionesMes.Where(r => r.TipoReunion == TipoReunion.Publica).ToList();
            var reunionesServicio = reunionesMes.Where(r => r.TipoReunion == TipoReunion.EntreSemana).ToList();
            var rpAnterior = reunionesAnterior.Where(r => r.TipoReunion == TipoReunion.Publica).ToList();
            var rsAnterior = reunionesAnterior.Where(r => r.TipoReunion == TipoReunion.EntreSemana).ToList();

            var promedioRpActual = reunionesPublicas.Any() ? reunionesPublicas.Average(r => r.Total) : 0;
            var promedioRsActual = reunionesServicio.Any() ? reunionesServicio.Average(r => r.Total) : 0;
            var promedioRpAnterior = rpAnterior.Any() ? rpAnterior.Average(r => r.Total) : 0;
            var promedioRsAnterior = rsAnterior.Any() ? rsAnterior.Average(r => r.Total) : 0;

            // ── Distribución por tipo (pastel) ────────────────────────────────
            var distribucion = new List<DistribucionTipoDto>
            {
                new() { Tipo = "Publicador",           Cantidad = publicadoresActivos.Count(p => p.Tipo == TipoPublicador.Publicador) },
                new() { Tipo = "No Bautizado",         Cantidad = publicadoresActivos.Count(p => p.Tipo == TipoPublicador.NoBautizado) },
                new() { Tipo = "Precursor Auxiliar",   Cantidad = publicadoresActivos.Count(p => p.Tipo == TipoPublicador.PrecursorAuxiliar) },
                new() { Tipo = "Precursor Regular",    Cantidad = publicadoresActivos.Count(p => p.Tipo == TipoPublicador.PrecursorRegular) }
            };

            // ── Historial semestral (barras) ──────────────────────────────────
            var historial = new List<HistorialMesDto>();
            for (var i = 5; i >= 0; i--)
            {
                var fecha = primerDia.AddMonths(-i);
                var informes = i == 0
                    ? informesMes
                    : await _datInforme.GetByMesAsync(fecha.Year, fecha.Month);

                historial.Add(new HistorialMesDto
                {
                    Ano = fecha.Year,
                    Mes = _nombresMeses[fecha.Month - 1],
                    Publicadores = informes.Count(x =>
                        x.Tipo == TipoPublicador.Publicador || x.Tipo == TipoPublicador.NoBautizado),
                    PrecursoresAuxiliares = informes.Count(x => x.Tipo == TipoPublicador.PrecursorAuxiliar),
                    PrecursoresRegulares = informes.Count(x => x.Tipo == TipoPublicador.PrecursorRegular)
                });
            }

            // ── Armar respuesta ───────────────────────────────────────────────
            var dashboard = new DashboardDto
            {
                Ano = ano,
                Mes = mes,
                TotalPublicadoresActivos = publicadoresActivos.Count,
                TotalInactivos = publicadoresActivos.Count(p => p.Inactivo),
                Publicadores = new CardInformesDto
                {
                    CantidadInformes = infPublicadores.Count,
                    Variacion = CalcularVariacion(infPubAnterior.Count, infPublicadores.Count)
                },
                PrecursoresAuxiliares = new CardInformesDto
                {
                    CantidadInformes = infAuxiliares.Count,
                    Variacion = CalcularVariacion(infAuxAnterior.Count, infAuxiliares.Count)
                },
                PrecursoresRegulares = new CardInformesDto
                {
                    CantidadInformes = infRegulares.Count,
                    Variacion = CalcularVariacion(infRegAnterior.Count, infRegulares.Count)
                },
                ReunionesPublicas = new CardReunionDto
                {
                    CantidadReuniones = reunionesPublicas.Count,
                    Promedio = Math.Round(promedioRpActual, 1),
                    Variacion = CalcularVariacion(promedioRpAnterior, promedioRpActual)
                },
                ReunionesServicio = new CardReunionDto
                {
                    CantidadReuniones = reunionesServicio.Count,
                    Promedio = Math.Round(promedioRsActual, 1),
                    Variacion = CalcularVariacion(promedioRsAnterior, promedioRsActual)
                },
                DistribucionPorTipo = distribucion,
                HistorialSemestral = historial
            };

            return ApiResponse<DashboardDto>.Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener dashboard: {Ano}/{Mes}.", ano, mes);
            return ApiResponse<DashboardDto>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    private static double CalcularVariacion(double valorAnterior, double valorActual)
    {
        if (valorAnterior == 0) return valorActual > 0 ? 100 : 0;
        return Math.Round(((valorActual - valorAnterior) / valorAnterior) * 100, 1);
    }
}