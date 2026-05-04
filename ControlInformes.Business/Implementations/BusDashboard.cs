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

    //public async Task<ApiResponse<DashboardDto>> GetDashboardAsync(int ano, int mes)
    //{
    //    try
    //    {
    //        if (ano < 1 || mes < 1 || mes > 12)
    //            return ApiResponse<DashboardDto>.Fail("Año o mes inválido.", ErrorCatalog.ValidacionFallida, 400);

    //        // Cargar datos del mes actual
    //        var publicadoresActivos = await _datPublicador.GetActivosAsync();
    //        var informesMes = await _datInforme.GetByMesAsync(ano, mes);

    //        var primerDia = new DateTime(ano, mes, 1);
    //        var ultimoDia = primerDia.AddMonths(1).AddDays(-1);
    //        var asistenciasMes = await _datAsistencia.GetByRangoAsync(primerDia, ultimoDia);

    //        // Cargar datos del mes anterior (para variaciones)
    //        var fechaAnterior = primerDia.AddMonths(-1);
    //        var informesMesAnterior = await _datInforme.GetByMesAsync(fechaAnterior.Year, fechaAnterior.Month);
    //        var primerDiaAnterior = new DateTime(fechaAnterior.Year, fechaAnterior.Month, 1);
    //        var ultimoDiaAnterior = primerDiaAnterior.AddMonths(1).AddDays(-1);
    //        var asistenciasMesAnterior = await _datAsistencia.GetByRangoAsync(primerDiaAnterior, ultimoDiaAnterior);

    //        // KPIs
    //        var kpis = new KpisDto
    //        {
    //            TotalPublicadoresActivos = publicadoresActivos.Count,
    //            InformesRecibidos = informesMes.Count,
    //            TotalCursosBiblicos = informesMes.Sum(i => i.CursosBiblicos),
    //            TotalHorasPrecursores = informesMes.Where(i => i.Horas.HasValue).Sum(i => i.Horas!.Value),
    //            PromedioAsistencia = asistenciasMes.Count > 0 ? Math.Round(asistenciasMes.Average(a => a.Cantidad), 1) : 0
    //        };

    //        // Tipos de publicador (activos)
    //        var tiposPublicador = new TiposPublicadorDto
    //        {
    //            Publicadores = publicadoresActivos.Count(p =>
    //                p.Tipo == TipoPublicador.Publicador || p.Tipo == TipoPublicador.NoBautizado),
    //            PrecursoresAuxiliares = publicadoresActivos.Count(p => p.Tipo == TipoPublicador.PrecursorAuxiliar),
    //            PrecursoresRegulares = publicadoresActivos.Count(p => p.Tipo == TipoPublicador.PrecursorRegular)
    //        };

    //        // Distribución por categoría
    //        var infPublicadores = informesMes.Where(i =>
    //            i.Tipo == TipoPublicador.Publicador || i.Tipo == TipoPublicador.NoBautizado).ToList();
    //        var infAuxiliares = informesMes.Where(i => i.Tipo == TipoPublicador.PrecursorAuxiliar).ToList();
    //        var infRegulares = informesMes.Where(i => i.Tipo == TipoPublicador.PrecursorRegular).ToList();

    //        var distribucion = new List<DistribucionDto>
    //        {
    //            new()
    //            {
    //                Tipo = "Publicadores",
    //                Informes = infPublicadores.Count,
    //                Cursos = infPublicadores.Sum(i => i.CursosBiblicos),
    //                Horas = 0
    //            },
    //            new()
    //            {
    //                Tipo = "PrecursoresAuxiliares",
    //                Informes = infAuxiliares.Count,
    //                Cursos = infAuxiliares.Sum(i => i.CursosBiblicos),
    //                Horas = infAuxiliares.Where(i => i.Horas.HasValue).Sum(i => i.Horas!.Value)
    //            },
    //            new()
    //            {
    //                Tipo = "PrecursoresRegulares",
    //                Informes = infRegulares.Count,
    //                Cursos = infRegulares.Sum(i => i.CursosBiblicos),
    //                Horas = infRegulares.Where(i => i.Horas.HasValue).Sum(i => i.Horas!.Value)
    //            }
    //        };

    //        // Historial semestral (6 meses hacia atrás incluyendo el actual)
    //        var historial = new List<HistorialMesDto>();
    //        for (var i = 5; i >= 0; i--)
    //        {
    //            var fecha = primerDia.AddMonths(-i);
    //            var informesPeriodo = i == 0
    //                ? informesMes
    //                : await _datInforme.GetByMesAsync(fecha.Year, fecha.Month);

    //            historial.Add(new HistorialMesDto
    //            {
    //                Mes = _nombresMeses[fecha.Month - 1],
    //                Ano = fecha.Year,
    //                PublicadoresActivos = publicadoresActivos.Count,
    //                Informes = informesPeriodo.Count,
    //                Cursos = informesPeriodo.Sum(x => x.CursosBiblicos),
    //                Horas = informesPeriodo.Where(x => x.Horas.HasValue).Sum(x => x.Horas!.Value)
    //            });
    //        }

    //        // Variaciones vs mes anterior
    //        var promedioAnterior = asistenciasMesAnterior.Count > 0
    //            ? asistenciasMesAnterior.Average(a => a.Cantidad)
    //            : 0;

    //        var variaciones = new VariacionesDto
    //        {
    //            CambioInformes = CalcularVariacion(informesMesAnterior.Count, informesMes.Count),
    //            CambioCursos = CalcularVariacion(
    //                informesMesAnterior.Sum(i => i.CursosBiblicos),
    //                informesMes.Sum(i => i.CursosBiblicos)),
    //            CambioHoras = CalcularVariacion(
    //                informesMesAnterior.Where(i => i.Horas.HasValue).Sum(i => i.Horas!.Value),
    //                informesMes.Where(i => i.Horas.HasValue).Sum(i => i.Horas!.Value)),
    //            CambioAsistencia = CalcularVariacion(promedioAnterior, kpis.PromedioAsistencia)
    //        };

    //        var dashboard = new DashboardDto
    //        {
    //            Ano = ano,
    //            Mes = mes,
    //            Kpis = kpis,
    //            TiposPublicador = tiposPublicador,
    //            Distribucion = distribucion,
    //            HistorialSemestral = historial,
    //            Variaciones = variaciones
    //        };

    //        return ApiResponse<DashboardDto>.Ok(dashboard);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error al obtener dashboard: {Ano}/{Mes}.", ano, mes);
    //        return ApiResponse<DashboardDto>.Error(ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
    //    }
    //}

    private static double CalcularVariacion(double valorAnterior, double valorActual)
    {
        if (valorAnterior == 0) return valorActual > 0 ? 100 : 0;
        return Math.Round(((valorActual - valorAnterior) / valorAnterior) * 100, 1);
    }
}