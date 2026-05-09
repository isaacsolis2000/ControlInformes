using AutoMapper;
using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using ControlInformes.Data.Interfaces;
using ControlInformes.Domain.Entities;
using ControlInformes.Domain.Enums;
using ControlInformes.Utils;
using iText.Forms;
using iText.Kernel.Pdf;
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
                // En RegistrarAsync — agregar antes de la validación de duplicado:
                var errores = ValidarAsistencia(dto.CantidadPresencial, dto.CantidadVirtual);
                if (errores.Count > 0)
                    return ApiResponse<Guid>.Fail("Errores de validación.", ErrorCatalog.ValidacionFallida, 400, errores);

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
                // En ActualizarAsync — agregar después de verificar que existe:
                var errores = ValidarAsistencia(dto.CantidadPresencial, dto.CantidadVirtual);
                if (errores.Count > 0)
                    return ApiResponse<string>.Fail("Errores de validación.", ErrorCatalog.ValidacionFallida, 400, errores);

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

    // Agregar este helper privado en BusAsistencia:
    private static List<string> ValidarAsistencia(int cantidadPresencial, int cantidadVirtual)
    {
        var errores = new List<string>();

        if (cantidadPresencial < 0)
            errores.Add("La cantidad presencial no puede ser negativa.");

        if (cantidadVirtual < 0)
            errores.Add("La cantidad virtual no puede ser negativa.");

        return errores;
    }

    public async Task<ApiResponse<byte[]>> DescargarTarjetaReunionesAsync(int anoServicio)
    {
        try
        {
            var rutaTemplate = Path.Combine(
                AppContext.BaseDirectory, "Template", "Templete_Tarjeta_Reuniones.pdf");

            if (!File.Exists(rutaTemplate))
                return ApiResponse<byte[]>.Error(
                    "Template de tarjeta de reuniones no encontrado.", ErrorCatalog.ArchivoInvalido);

            // ── Definición de años ────────────────────────────────────────────
            // anoServicio=2026:
            //   Columna izquierda (2026): Sep 2025 → Ago 2026
            //   Columna derecha   (2027): Sep 2026 → Ago 2027

            int ano1Inicio = anoServicio - 1; // Sep-Dic izquierda: 2025
            int ano1Fin = anoServicio;     // Ene-Ago izquierda: 2026
            int ano2Inicio = anoServicio;     // Sep-Dic derecha:   2026
            int ano2Fin = anoServicio + 1; // Ene-Ago derecha:   2027

            using var ms = new MemoryStream();
            using var reader = new PdfReader(rutaTemplate);
            using var writer = new PdfWriter(ms);
            using var pdfDoc = new PdfDocument(reader, writer);
            var form = PdfAcroForm.GetAcroForm(pdfDoc, false);

            // ── Encabezado ────────────────────────────────────────────────────
            // El PDF muestra el año de servicio (el que termina)
            // Izquierda = anoServicio (2026), Derecha = anoServicio+1 (2027)
            SetCampo(form, "Service Year_1", anoServicio.ToString());       // ES izq: 2026
            SetCampo(form, "Service Year_2", (anoServicio + 1).ToString()); // ES der: 2027
            SetCampo(form, "Service Year_3", anoServicio.ToString());       // FS izq: 2026
            SetCampo(form, "Service Year_4", (anoServicio + 1).ToString()); // FS der: 2027

            // ── Configuración de meses ────────────────────────────────────────
            // Año 1 (izquierda): Sep 2025 → Ago 2026
            var mesesAno1 = new[]
            {
            (Indice: 1,  Mes: 9,  Ano: ano1Inicio), // Sep 2025
            (Indice: 2,  Mes: 10, Ano: ano1Inicio), // Oct 2025
            (Indice: 3,  Mes: 11, Ano: ano1Inicio), // Nov 2025
            (Indice: 4,  Mes: 12, Ano: ano1Inicio), // Dic 2025
            (Indice: 5,  Mes: 1,  Ano: ano1Fin),    // Ene 2026
            (Indice: 6,  Mes: 2,  Ano: ano1Fin),    // Feb 2026
            (Indice: 7,  Mes: 3,  Ano: ano1Fin),    // Mar 2026
            (Indice: 8,  Mes: 4,  Ano: ano1Fin),    // Abr 2026
            (Indice: 9,  Mes: 5,  Ano: ano1Fin),    // May 2026
            (Indice: 10, Mes: 6,  Ano: ano1Fin),    // Jun 2026
            (Indice: 11, Mes: 7,  Ano: ano1Fin),    // Jul 2026
            (Indice: 12, Mes: 8,  Ano: ano1Fin)     // Ago 2026
        };

            // Año 2 (derecha): Sep 2026 → Ago 2027
            var mesesAno2 = new[]
            {
            (Indice: 1,  Mes: 9,  Ano: ano2Inicio), // Sep 2026
            (Indice: 2,  Mes: 10, Ano: ano2Inicio), // Oct 2026
            (Indice: 3,  Mes: 11, Ano: ano2Inicio), // Nov 2026
            (Indice: 4,  Mes: 12, Ano: ano2Inicio), // Dic 2026
            (Indice: 5,  Mes: 1,  Ano: ano2Fin),    // Ene 2027
            (Indice: 6,  Mes: 2,  Ano: ano2Fin),    // Feb 2027
            (Indice: 7,  Mes: 3,  Ano: ano2Fin),    // Mar 2027
            (Indice: 8,  Mes: 4,  Ano: ano2Fin),    // Abr 2027
            (Indice: 9,  Mes: 5,  Ano: ano2Fin),    // May 2027
            (Indice: 10, Mes: 6,  Ano: ano2Fin),    // Jun 2027
            (Indice: 11, Mes: 7,  Ano: ano2Fin),    // Jul 2027
            (Indice: 12, Mes: 8,  Ano: ano2Fin)     // Ago 2027
        };

            double totalES1 = 0, totalFS1 = 0;
            double totalES2 = 0, totalFS2 = 0;
            int cntES1 = 0, cntFS1 = 0;
            int cntES2 = 0, cntFS2 = 0;

            // ── Año 1: bloques 1=EntreSemana, 3=FinSemana ────────────────────
            foreach (var (indice, mes, ano) in mesesAno1)
            {
                // Entre semana
                var es = await _datAsistencia.GetByMesYTipoAsync(ano, mes, TipoReunion.EntreSemana);
                int numES = es.Count;
                int sumES = es.Sum(r => r.Total);
                double pES = numES > 0 ? Math.Round((double)sumES / numES, 1) : 0;

                SetCampo(form, $"1-Meeting_{indice}", numES > 0 ? numES.ToString() : string.Empty);
                SetCampo(form, $"1-Attendance_{indice}", sumES > 0 ? sumES.ToString() : string.Empty);
                SetCampo(form, $"1-Average_{indice}", pES > 0 ? pES.ToString("F1") : string.Empty);
                if (numES > 0) { totalES1 += pES; cntES1++; }

                // Fin de semana
                var fs = await _datAsistencia.GetByMesYTipoAsync(ano, mes, TipoReunion.Publica);
                int numFS = fs.Count;
                int sumFS = fs.Sum(r => r.Total);
                double pFS = numFS > 0 ? Math.Round((double)sumFS / numFS, 1) : 0;

                SetCampo(form, $"3-Meeting_{indice}", numFS > 0 ? numFS.ToString() : string.Empty);
                SetCampo(form, $"3-Attendance_{indice}", sumFS > 0 ? sumFS.ToString() : string.Empty);
                SetCampo(form, $"3-Average_{indice}", pFS > 0 ? pFS.ToString("F1") : string.Empty);
                if (numFS > 0) { totalFS1 += pFS; cntFS1++; }
            }

            // ── Año 2: bloques 2=EntreSemana, 4=FinSemana ────────────────────
            foreach (var (indice, mes, ano) in mesesAno2)
            {
                // Entre semana
                var es = await _datAsistencia.GetByMesYTipoAsync(ano, mes, TipoReunion.EntreSemana);
                int numES = es.Count;
                int sumES = es.Sum(r => r.Total);
                double pES = numES > 0 ? Math.Round((double)sumES / numES, 1) : 0;

                SetCampo(form, $"2-Meeting_{indice}", numES > 0 ? numES.ToString() : string.Empty);
                SetCampo(form, $"2-Attendance_{indice}", sumES > 0 ? sumES.ToString() : string.Empty);
                SetCampo(form, $"2-Average_{indice}", pES > 0 ? pES.ToString("F1") : string.Empty);
                if (numES > 0) { totalES2 += pES; cntES2++; }

                // Fin de semana
                var fs = await _datAsistencia.GetByMesYTipoAsync(ano, mes, TipoReunion.Publica);
                int numFS = fs.Count;
                int sumFS = fs.Sum(r => r.Total);
                double pFS = numFS > 0 ? Math.Round((double)sumFS / numFS, 1) : 0;

                SetCampo(form, $"4-Meeting_{indice}", numFS > 0 ? numFS.ToString() : string.Empty);
                SetCampo(form, $"4-Attendance_{indice}", sumFS > 0 ? sumFS.ToString() : string.Empty);
                SetCampo(form, $"4-Average_{indice}", pFS > 0 ? pFS.ToString("F1") : string.Empty);
                if (numFS > 0) { totalFS2 += pFS; cntFS2++; }
            }

            // ── Promedios totales ─────────────────────────────────────────────
            SetCampo(form, "1-Average_Total",
                cntES1 > 0 ? Math.Round(totalES1 / cntES1, 1).ToString("F1") : string.Empty);
            SetCampo(form, "3-Average_Total",
                cntFS1 > 0 ? Math.Round(totalFS1 / cntFS1, 1).ToString("F1") : string.Empty);
            SetCampo(form, "2-Average_Total",
                cntES2 > 0 ? Math.Round(totalES2 / cntES2, 1).ToString("F1") : string.Empty);
            SetCampo(form, "4-Average_Total",
                cntFS2 > 0 ? Math.Round(totalFS2 / cntFS2, 1).ToString("F1") : string.Empty);

            pdfDoc.Close();
            return ApiResponse<byte[]>.Ok(ms.ToArray(), "Tarjeta de reuniones generada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar tarjeta de reuniones: {AnoServicio}.", anoServicio);
            return ApiResponse<byte[]>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    private static void SetCampo(PdfAcroForm form, string nombre, string valor)
    {
        var campo = form.GetField(nombre);
        if (campo != null)
            campo.SetValue(valor);
    }
}