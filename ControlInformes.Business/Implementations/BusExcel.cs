using ClosedXML.Excel;
using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using ControlInformes.Data.Interfaces;
using ControlInformes.Domain.Entities;
using ControlInformes.Domain.Enums;
using ControlInformes.Utils;
using Microsoft.Extensions.Logging;

namespace ControlInformes.Business.Implementations;

public class BusExcel : IBusExcel
{
    private readonly IExcelService _excelService;
    private readonly IDatPublicador _datPublicador;
    private readonly IDatInformeMensual _datInforme;
    private readonly IDatGrupo _datGrupo;
    private readonly ILogger<BusExcel> _logger;

    public BusExcel(
        IExcelService excelService,
        IDatPublicador datPublicador,
        IDatInformeMensual datInforme,
        IDatGrupo datGrupo,
        ILogger<BusExcel> logger)
    {
        _excelService = excelService;
        _datPublicador = datPublicador;
        _datInforme = datInforme;
        _datGrupo = datGrupo;
        _logger = logger;
    }

    public async Task<ApiResponse<ImportacionResultadoDto>> ImportarAsync(
        Stream archivoStream, int ano, int mes, Guid idGrupo)
    {
        try
        {
            // Validar grupo
            var grupo = await _datGrupo.GetConCapitanAsync(idGrupo);
            if (grupo == null)
                return ApiResponse<ImportacionResultadoDto>.NotFound(
                    $"Grupo con Id ({idGrupo}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            var resultado = new ImportacionResultadoDto();
            var filas = _excelService.LeerInformes(archivoStream);
            var nuevos = new List<InformeMensual>();

            foreach (var fila in filas)
            {
                resultado.TotalProcesados++;
                try
                {
                    // Validar nombre
                    if (string.IsNullOrWhiteSpace(fila.Nombre))
                    {
                        resultado.Fallidos++;
                        resultado.Errores.Add($"Fila {resultado.TotalProcesados}: Nombre vacío.");
                        continue;
                    }

                    // Validar tipo
                    if (!Enum.TryParse<TipoPublicador>(fila.Tipo, true, out var tipo))
                    {
                        resultado.Fallidos++;
                        resultado.Errores.Add($"Fila {resultado.TotalProcesados}: Tipo '{fila.Tipo}' no válido.");
                        continue;
                    }

                    // Buscar publicador dentro del grupo
                    var publicador = grupo.Publicadores
                        .FirstOrDefault(p => p.NombreCompleto.Trim().ToLower() == fila.Nombre.Trim().ToLower());

                    if (publicador == null)
                    {
                        resultado.Fallidos++;
                        resultado.Errores.Add($"Fila {resultado.TotalProcesados}: '{fila.Nombre}' no pertenece al grupo.");
                        continue;
                    }

                    // Validar reglas de negocio
                    var errores = ValidarFila(fila, tipo, resultado.TotalProcesados);
                    if (errores.Count > 0)
                    {
                        resultado.Fallidos++;
                        resultado.Errores.AddRange(errores);
                        continue;
                    }

                    var (horas, cursos) = LimpiarCampos(tipo, fila.Inactivo, fila.Participo, fila.Horas, fila.Cursos);

                    // Upsert
                    var existente = await _datInforme.GetByPublicadorMesAsync(publicador.IdPublicador, ano, mes);
                    if (existente != null)
                    {
                        existente.Participo = fila.Participo;
                        existente.CursosBiblicos = cursos;
                        existente.Horas = horas;
                        existente.Tipo = tipo;
                        existente.Inactivo = fila.Inactivo;
                        existente.Observacion = fila.Observacion;
                        _datInforme.Update(existente);
                    }
                    else
                    {
                        nuevos.Add(new InformeMensual
                        {
                            IdInformeMensual = Guid.NewGuid(),
                            IdPublicador = publicador.IdPublicador,
                            Ano = ano,
                            Mes = mes,
                            Participo = fila.Participo,
                            CursosBiblicos = cursos,
                            Horas = horas,
                            Tipo = tipo,
                            Inactivo = fila.Inactivo,
                            Observacion = fila.Observacion
                        });
                    }

                    resultado.Exitosos++;
                }
                catch (Exception ex)
                {
                    resultado.Fallidos++;
                    resultado.Errores.Add($"Fila {resultado.TotalProcesados}: {ex.Message}");
                }
            }

            if (nuevos.Any())
                await _datInforme.AddRangeAsync(nuevos);

            await _datInforme.SaveChangesAsync();

            _logger.LogInformation("Importación: {Exitosos}/{Total}", resultado.Exitosos, resultado.TotalProcesados);
            return ApiResponse<ImportacionResultadoDto>.Ok(resultado, "Importación completada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado durante la importación Excel.");
            return ApiResponse<ImportacionResultadoDto>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<byte[]>> GenerarTemplateAsync(Guid idGrupo)
    {
        try
        {
            var grupos = await _datGrupo.GetConMiembrosAsync();
            var grupo = grupos.FirstOrDefault(g => g.IdGrupo == idGrupo);

            if (grupo == null)
                return ApiResponse<byte[]>.NotFound(
                    $"Grupo con Id ({idGrupo}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            var nombres = grupo.Publicadores
                .OrderBy(p => p.NombreCompleto)
                .Select(p => p.NombreCompleto)
                .ToList();

            var bytes = _excelService.GenerarTemplate(nombres);
            return ApiResponse<byte[]>.Ok(bytes, "Template generado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar template Excel.");
            return ApiResponse<byte[]>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<byte[]>> GenerarListadoPublicadoresAsync()
    {
        try
        {
            var publicadores = await _datPublicador.GetAllAsync();
            var ordenados = publicadores
                .OrderBy(p => p.IdGrupo == null ? 1 : 0)
                .ThenBy(p => p.NombreCompleto)
                .ToList();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Publicadores");

            // Encabezados
            ws.Cell(1, 1).Value = "Nombre";
            ws.Cell(1, 2).Value = "Tipo";
            ws.Cell(1, 3).Value = "Grupo";
            ws.Cell(1, 4).Value = "Inactivo";
            ws.Cell(1, 5).Value = "Fecha Nacimiento";
            ws.Cell(1, 6).Value = "Fecha Bautismo";

            var header = ws.Range(1, 1, 1, 6);
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
            header.Style.Font.FontColor = XLColor.White;

            // Datos
            for (int i = 0; i < ordenados.Count; i++)
            {
                var p = ordenados[i];
                int fila = i + 2;
                ws.Cell(fila, 1).Value = p.NombreCompleto;
                ws.Cell(fila, 2).Value = p.Tipo.ToString();
                ws.Cell(fila, 3).Value = p.Grupo?.Nombre ?? string.Empty;
                ws.Cell(fila, 4).Value = p.Inactivo ? "Sí" : "No";
                ws.Cell(fila, 5).SetValue(p.FechaNacimiento.HasValue? p.FechaNacimiento.Value.ToString("dd/MM/yyyy"): string.Empty);
                ws.Cell(fila, 6).SetValue(p.FechaBautismo.HasValue
                    ? p.FechaBautismo.Value.ToString("dd/MM/yyyy")
                    : string.Empty);
            }

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ApiResponse<byte[]>.Ok(ms.ToArray(), "Listado generado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar listado de publicadores.");
            return ApiResponse<byte[]>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<byte[]>> GenerarListadoGruposAsync()
    {
        try
        {
            var grupos = await _datGrupo.GetConMiembrosAsync();
            var ordenados = grupos.OrderBy(g => g.Nombre).ToList();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Grupos");

            // Encabezados
            ws.Cell(1, 1).Value = "Grupo";
            ws.Cell(1, 2).Value = "Capitán";
            ws.Cell(1, 3).Value = "Publicador";
            ws.Cell(1, 4).Value = "Tipo";
            ws.Cell(1, 5).Value = "Inactivo";

            var header = ws.Range(1, 1, 1, 5);
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
            header.Style.Font.FontColor = XLColor.White;

            int fila = 2;
            foreach (var grupo in ordenados)
            {
                foreach (var pub in grupo.Publicadores.OrderBy(p => p.NombreCompleto))
                {
                    ws.Cell(fila, 1).Value = grupo.Nombre;
                    ws.Cell(fila, 2).Value = grupo.Capitan?.NombreCompleto ?? string.Empty;
                    ws.Cell(fila, 3).Value = pub.NombreCompleto;
                    ws.Cell(fila, 4).Value = pub.Tipo.ToString();
                    ws.Cell(fila, 5).Value = pub.Inactivo ? "Sí" : "No";
                    fila++;
                }
            }

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ApiResponse<byte[]>.Ok(ms.ToArray(), "Listado generado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar listado de grupos.");
            return ApiResponse<byte[]>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    // ── Helpers privados ─────────────────────────────────────────────────────

    private static List<string> ValidarFila(ExcelInformeRow fila, TipoPublicador tipo, int numFila)
    {
        var errores = new List<string>();
        var esPrecursor = tipo == TipoPublicador.PrecursorAuxiliar || tipo == TipoPublicador.PrecursorRegular;
        var esPublicador = tipo == TipoPublicador.Publicador || tipo == TipoPublicador.NoBautizado;

        if (fila.Inactivo)
            return errores; // Inactivo: no se valida nada más

        if (fila.Participo)
        {


            if (esPublicador && fila.Horas.HasValue && fila.Horas > 0)
                errores.Add($"Fila {numFila}: Los publicadores no registran horas.");

            if (fila.Cursos < 0)
                errores.Add($"Fila {numFila}: Los cursos no pueden ser negativos.");
        }

        return errores;
    }

    private static (int? horas, int cursos) LimpiarCampos(
        TipoPublicador tipo, bool inactivo, bool participo, int? horas, int cursos)
    {
        if (inactivo || !participo)
            return (null, 0);

        if (tipo == TipoPublicador.Publicador || tipo == TipoPublicador.NoBautizado)
            return (null, cursos);

        return (horas, cursos);
    }
}