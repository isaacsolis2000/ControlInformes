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

    // Orden Sep→Ago del año de servicio
    private static readonly (int Mes, string Nombre)[] _mesesServicio =
    [
        (9,  "Septiembre"),
        (10, "Octubre"),
        (11, "Noviembre"),
        (12, "Diciembre"),
        (1,  "Enero"),
        (2,  "Febrero"),
        (3,  "Marzo"),
        (4,  "Abril"),
        (5,  "Mayo"),
        (6,  "Junio"),
        (7,  "Julio"),
        (8,  "Agosto")
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

    public async Task<ApiResponse<DashboardDto>> GetDashboardAsync(int anoServicio, int? mes)
    {
        try
        {
            if (anoServicio < 2000 || anoServicio > 2100)
                return ApiResponse<DashboardDto>.Fail(
                    "Año de servicio inválido.", ErrorCatalog.ValidacionFallida, 400);

            if (mes.HasValue && (mes < 1 || mes > 12))
                return ApiResponse<DashboardDto>.Fail(
                    "Mes inválido.", ErrorCatalog.ValidacionFallida, 400);

            int anoInicio = anoServicio - 1;  // Sep-Dic de este año
            int anoFin    = anoServicio;       // Ene-Ago de este año

            // ── Publicadores ──────────────────────────────────────────────
            var publicadoresActivos = await _datPublicador.GetActivosAsync();

            // ── Cargar informes de todo el año de servicio (12 meses) ─────
            // Sep-Dic del anoInicio + Ene-Ago del anoFin
            var informesAno = new List<Domain.Entities.InformeMensual>();
            for (int m = 9; m <= 12; m++)
                informesAno.AddRange(await _datInforme.GetByMesAsync(anoInicio, m));
            for (int m = 1; m <= 8; m++)
                informesAno.AddRange(await _datInforme.GetByMesAsync(anoFin, m));

            // ── Filtrar informes para cards ───────────────────────────────
            // Si hay mes: solo ese mes | Si no: acumulado del año
            List<Domain.Entities.InformeMensual> informesFiltrados;
            List<Domain.Entities.InformeMensual> informesComparacion;

            if (mes.HasValue)
            {
                // Mes actual filtrado
                int anoDelMes = mes.Value >= 9 ? anoInicio : anoFin;
                informesFiltrados = informesAno
                    .Where(i => i.Mes == mes.Value && i.Ano == anoDelMes)
                    .ToList();

                // Mes anterior para variación
                int mesAnterior = mes.Value == 1 ? 12 : mes.Value - 1;
                int anoMesAnterior = mesAnterior >= 9 ? anoInicio : anoFin;
                informesComparacion = informesAno
                    .Where(i => i.Mes == mesAnterior && i.Ano == anoMesAnterior)
                    .ToList();
            }
            else
            {
                // Acumulado del año completo
                informesFiltrados = informesAno;
                informesComparacion = new List<Domain.Entities.InformeMensual>();
            }

            // ── Cards de publicadores ─────────────────────────────────────
            var infPublicadores = informesFiltrados
                .Where(i => i.Tipo == TipoPublicador.Publicador || i.Tipo == TipoPublicador.NoBautizado)
                .ToList();
            var infAuxiliares = informesFiltrados
                .Where(i => i.Tipo == TipoPublicador.PrecursorAuxiliar)
                .ToList();
            var infRegulares = informesFiltrados
                .Where(i => i.Tipo == TipoPublicador.PrecursorRegular)
                .ToList();

            var infPubComp = informesComparacion
                .Where(i => i.Tipo == TipoPublicador.Publicador || i.Tipo == TipoPublicador.NoBautizado)
                .ToList();
            var infAuxComp = informesComparacion
                .Where(i => i.Tipo == TipoPublicador.PrecursorAuxiliar)
                .ToList();
            var infRegComp = informesComparacion
                .Where(i => i.Tipo == TipoPublicador.PrecursorRegular)
                .ToList();

            // ── Cards de reuniones ────────────────────────────────────────
            List<Domain.Entities.Asistencia> reunionesFiltradas;
            List<Domain.Entities.Asistencia> reunionesComparacion;

            if (mes.HasValue)
            {
                int anoDelMes = mes.Value >= 9 ? anoInicio : anoFin;
                var (rMes, _) = await _datAsistencia.GetPaginadoAsync(
                    anoDelMes, mes.Value, null, 1, 100);
                reunionesFiltradas = rMes;

                int mesAnterior = mes.Value == 1 ? 12 : mes.Value - 1;
                int anoMesAnterior = mesAnterior >= 9 ? anoInicio : anoFin;
                var (rAnterior, _) = await _datAsistencia.GetPaginadoAsync(
                    anoMesAnterior, mesAnterior, null, 1, 100);
                reunionesComparacion = rAnterior;
            }
            else
            {
                // Cargar todas las reuniones del año de servicio
                reunionesFiltradas = new List<Domain.Entities.Asistencia>();
                reunionesComparacion = new List<Domain.Entities.Asistencia>();

                for (int m = 9; m <= 12; m++)
                {
                    var (r, _) = await _datAsistencia.GetPaginadoAsync(anoInicio, m, null, 1, 100);
                    reunionesFiltradas.AddRange(r);
                }
                for (int m = 1; m <= 8; m++)
                {
                    var (r, _) = await _datAsistencia.GetPaginadoAsync(anoFin, m, null, 1, 100);
                    reunionesFiltradas.AddRange(r);
                }
            }

            var reunionesPublicas = reunionesFiltradas
                .Where(r => r.TipoReunion == TipoReunion.Publica).ToList();
            var reunionesServicio = reunionesFiltradas
                .Where(r => r.TipoReunion == TipoReunion.EntreSemana).ToList();
            var rpComp = reunionesComparacion
                .Where(r => r.TipoReunion == TipoReunion.Publica).ToList();
            var rsComp = reunionesComparacion
                .Where(r => r.TipoReunion == TipoReunion.EntreSemana).ToList();

            var promedioRpActual = reunionesPublicas.Any() ? reunionesPublicas.Average(r => r.Total) : 0;
            var promedioRsActual = reunionesServicio.Any() ? reunionesServicio.Average(r => r.Total) : 0;
            var promedioRpAnterior = rpComp.Any() ? rpComp.Average(r => r.Total) : 0;
            var promedioRsAnterior = rsComp.Any() ? rsComp.Average(r => r.Total) : 0;

            // ── Distribución por tipo (pastel — siempre activos) ──────────
            var distribucion = new List<DistribucionTipoDto>
            {
                new() { Tipo = "Publicador",         Cantidad = publicadoresActivos.Count(p => p.Tipo == TipoPublicador.Publicador) },
                new() { Tipo = "No Bautizado",       Cantidad = publicadoresActivos.Count(p => p.Tipo == TipoPublicador.NoBautizado) },
                new() { Tipo = "Precursor Auxiliar", Cantidad = publicadoresActivos.Count(p => p.Tipo == TipoPublicador.PrecursorAuxiliar) },
                new() { Tipo = "Precursor Regular",  Cantidad = publicadoresActivos.Count(p => p.Tipo == TipoPublicador.PrecursorRegular) }
            };

            // ── Historial 12 meses Sep→Ago (siempre completo) ────────────
            var historial = new List<HistorialMesDto>();

            foreach (var (numMes, nombreMes) in _mesesServicio)
            {
                int anoDelMes = numMes >= 9 ? anoInicio : anoFin;
                var informesMes = informesAno
                    .Where(i => i.Mes == numMes && i.Ano == anoDelMes)
                    .ToList();

                historial.Add(new HistorialMesDto
                {
                    Ano = anoDelMes,
                    NumeroMes = numMes,
                    Mes = nombreMes,
                    Publicadores = informesMes.Count(x =>
                        x.Tipo == TipoPublicador.Publicador || x.Tipo == TipoPublicador.NoBautizado),
                    PrecursoresAuxiliares = informesMes.Count(x => x.Tipo == TipoPublicador.PrecursorAuxiliar),
                    PrecursoresRegulares = informesMes.Count(x => x.Tipo == TipoPublicador.PrecursorRegular),
                    EsMesFiltrado = mes.HasValue && mes.Value == numMes
                });
            }

            // ── Armar respuesta ───────────────────────────────────────────
            var dashboard = new DashboardDto
            {
                AnoServicioInicio = anoInicio,
                AnoServicioFin = anoFin,
                MesFiltrado = mes,
                TotalPublicadoresActivos = publicadoresActivos.Count,
                TotalInactivos = publicadoresActivos.Count(p => p.Inactivo),
                Publicadores = new CardInformesDto
                {
                    CantidadInformes = infPublicadores.Count,
                    Variacion = mes.HasValue
                        ? CalcularVariacion(infPubComp.Count, infPublicadores.Count)
                        : 0 // Sin variación en modo anual
                },
                PrecursoresAuxiliares = new CardInformesDto
                {
                    CantidadInformes = infAuxiliares.Count,
                    Variacion = mes.HasValue
                        ? CalcularVariacion(infAuxComp.Count, infAuxiliares.Count)
                        : 0
                },
                PrecursoresRegulares = new CardInformesDto
                {
                    CantidadInformes = infRegulares.Count,
                    Variacion = mes.HasValue
                        ? CalcularVariacion(infRegComp.Count, infRegulares.Count)
                        : 0
                },
                ReunionesPublicas = new CardReunionDto
                {
                    CantidadReuniones = reunionesPublicas.Count,
                    Promedio = Math.Round(promedioRpActual, 1),
                    Variacion = mes.HasValue
                        ? CalcularVariacion(promedioRpAnterior, promedioRpActual)
                        : 0
                },
                ReunionesServicio = new CardReunionDto
                {
                    CantidadReuniones = reunionesServicio.Count,
                    Promedio = Math.Round(promedioRsActual, 1),
                    Variacion = mes.HasValue
                        ? CalcularVariacion(promedioRsAnterior, promedioRsActual)
                        : 0
                },
                DistribucionPorTipo = distribucion,
                Historial12Meses = historial
            };

            return ApiResponse<DashboardDto>.Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener dashboard: AnoServicio={Ano}, Mes={Mes}.",
                anoServicio, mes);
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