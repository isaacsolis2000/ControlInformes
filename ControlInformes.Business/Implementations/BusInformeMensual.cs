using ClosedXML.Excel;
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
    private readonly IDatAsistencia _datAsistencia;
    private readonly IDatGrupo _datGrupo;
    private readonly ILogger<BusInformeMensual> _logger;

    public BusInformeMensual(
        IDatInformeMensual datInforme,
        IDatPublicador datPublicador,
        IDatAsistencia datAsistencia,
        IDatGrupo datGrupo,
        ILogger<BusInformeMensual> logger)
    {
        _datInforme = datInforme;
        _datPublicador = datPublicador;
        _datAsistencia = datAsistencia;
        _datGrupo = datGrupo;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<InformeMensualDto>>> GetPaginadoAsync(FiltroInformeDto filtro)
    {
        try
        {
            var (items, total) = await _datInforme.GetPaginadoAsync(
                filtro.Ano, filtro.Mes, filtro.IdGrupo, filtro.IdPublicador,
                filtro.Tipo, filtro.Inactivo, filtro.Pagina, filtro.TamanoPagina);

            var dtos = items.Select(i => new InformeMensualDto
            {
                IdInformeMensual = i.IdInformeMensual,
                IdPublicador = i.IdPublicador,
                NombrePublicador = i.Publicador.NombreCompleto,
                IdGrupo = i.Publicador.IdGrupo,
                NombreGrupo = i.Publicador.Grupo?.Nombre ?? string.Empty,
                Ano = i.Ano,
                Mes = i.Mes,
                Participo = i.Participo,
                CursosBiblicos = i.CursosBiblicos,
                Horas = i.Horas,
                Tipo = i.Tipo,
                TipoDescripcion = i.Tipo.ToString(),
                Inactivo = i.Inactivo,
                Observacion = i.Observacion
            }).ToList();

            var result = new PagedResult<InformeMensualDto>
            {
                Items = dtos,
                TotalRegistros = total,
                Pagina = filtro.Pagina,
                TamanoPagina = filtro.TamanoPagina
            };

            return ApiResponse<PagedResult<InformeMensualDto>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener informes paginados.");
            return ApiResponse<PagedResult<InformeMensualDto>>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<TotalInformeDto>> GetTotalAsync(int ano, int mes)
    {
        try
        {
            var informes = await _datInforme.GetByMesAsync(ano, mes);
            var grupos = await _datGrupo.GetAllAsync();
            var publicadoresActivos = await _datPublicador.GetActivosAsync();

            // ── Reuniones separadas por tipo ──────────────────────────────────
            var (reuniones, _) = await _datAsistencia.GetPaginadoAsync(ano, mes, null, 1, 100);

            var reunionesES = reuniones
                .Where(r => r.TipoReunion == TipoReunion.EntreSemana).ToList();
            var reunionesFS = reuniones
                .Where(r => r.TipoReunion == TipoReunion.Publica).ToList();

            var promedioES = reunionesES.Any()
                ? Math.Round(reunionesES.Average(r => r.Total), 1) : 0;
            var promedioFS = reunionesFS.Any()
                ? Math.Round(reunionesFS.Average(r => r.Total), 1) : 0;

            // ── Informes ──────────────────────────────────────────────────────
            var informesActivos = informes.Where(i => !i.Inactivo).ToList();

            var infoPublicadores = informesActivos
                .Where(i => i.Tipo == TipoPublicador.Publicador || i.Tipo == TipoPublicador.NoBautizado)
                .ToList();
            var infoAuxiliares = informesActivos
                .Where(i => i.Tipo == TipoPublicador.PrecursorAuxiliar).ToList();
            var infoRegulares = informesActivos
                .Where(i => i.Tipo == TipoPublicador.PrecursorRegular).ToList();

            // ── Grupos sin informe ────────────────────────────────────────────
            var gruposSinInforme = grupos
                .Where(g => !informes.Any(i => i.Publicador.IdGrupo == g.IdGrupo))
                .Select(g => g.Nombre)
                .ToList();

            // ── Publicadores sin informe agrupados por grupo ──────────────────
            var idsConInforme = informes.Select(i => i.IdPublicador).ToHashSet();

            var publicadoresSinInforme = publicadoresActivos
                .Where(p => !idsConInforme.Contains(p.IdPublicador))
                .ToList();

            // Agrupar por grupo — sin grupo va en clave "Sin grupo"
            var sinInformePorGrupo = new Dictionary<string, List<string>>();

            foreach (var pub in publicadoresSinInforme.OrderBy(p => p.NombreCompleto))
            {
                // Buscar nombre del grupo
                var nombreGrupo = pub.IdGrupo.HasValue
                    ? grupos.FirstOrDefault(g => g.IdGrupo == pub.IdGrupo.Value)?.Nombre ?? "Sin grupo"
                    : "Sin grupo";

                if (!sinInformePorGrupo.ContainsKey(nombreGrupo))
                    sinInformePorGrupo[nombreGrupo] = new List<string>();

                sinInformePorGrupo[nombreGrupo].Add(pub.NombreCompleto);
            }

            // Ordenar: grupos primero (alfabético), "Sin grupo" al final
            var sinInformeOrdenado = sinInformePorGrupo
                .OrderBy(kvp => kvp.Key == "Sin grupo" ? 1 : 0)
                .ThenBy(kvp => kvp.Key)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // ── Armar respuesta ───────────────────────────────────────────────
            var total = new TotalInformeDto
            {
                Ano = ano,
                Mes = mes,
                TotalPublicadores = informesActivos
                    .Select(i => i.IdPublicador)
                    .Distinct()
                    .Count(),
                PromedioReunionEntreSemana = new PromedioReunionDto
                {
                    HayReuniones = reunionesES.Any(),
                    CantidadReuniones = reunionesES.Count,
                    Promedio = promedioES
                },
                PromedioReunionFinSemana = new PromedioReunionDto
                {
                    HayReuniones = reunionesFS.Any(),
                    CantidadReuniones = reunionesFS.Count,
                    Promedio = promedioFS
                },
                Publicadores = new ResumenTipoDto
                {
                    CantidadInformes = infoPublicadores.Count,
                    CursosBiblicos = infoPublicadores.Sum(i => i.CursosBiblicos)
                },
                PrecursoresAuxiliares = new ResumenPrecursorDto
                {
                    CantidadInformes = infoAuxiliares.Count,
                    Horas = infoAuxiliares.Sum(i => i.Horas ?? 0),
                    CursosBiblicos = infoAuxiliares.Sum(i => i.CursosBiblicos)
                },
                PrecursoresRegulares = new ResumenPrecursorDto
                {
                    CantidadInformes = infoRegulares.Count,
                    Horas = infoRegulares.Sum(i => i.Horas ?? 0),
                    CursosBiblicos = infoRegulares.Sum(i => i.CursosBiblicos)
                },
                Pendientes = new PendientesDto
                {
                    ReunionesRegistradas = reuniones.Any(),
                    GruposSinInforme = gruposSinInforme,
                    PublicadoresSinInformePorGrupo = sinInformeOrdenado
                }
            };

            return ApiResponse<TotalInformeDto>.Ok(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener totales del mes {Ano}/{Mes}.", ano, mes);
            return ApiResponse<TotalInformeDto>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<Guid>> RegistrarAsync(RegistrarInformeDto dto)
    {
        try
        {
            var publicador = await _datPublicador.GetByIdAsync(dto.IdPublicador);
            if (publicador == null)
                return ApiResponse<Guid>.NotFound(
                    $"Publicador con Id ({dto.IdPublicador}) no encontrado.",
                    ErrorCatalog.EntidadNoEncontrada);

            var tipoFinal = dto.TipoInformativo ?? publicador.Tipo;

            var errores = ValidarInforme(dto.Horas, dto.CursosBiblicos, dto.Ano, dto.Mes);
            if (errores.Count > 0)
                return ApiResponse<Guid>.Fail("Errores de validación.", ErrorCatalog.ValidacionFallida, 400, errores);

            var (horas, cursos) = LimpiarCamposPorTipo(tipoFinal, dto.Inactivo, dto.Participo, dto.Horas, dto.CursosBiblicos);

            var existente = await _datInforme.GetByPublicadorMesAsync(dto.IdPublicador, dto.Ano, dto.Mes);
            if (existente != null)
            {
                existente.Participo = dto.Participo;
                existente.CursosBiblicos = cursos;
                existente.Horas = horas;
                existente.Tipo = tipoFinal;
                existente.Inactivo = dto.Inactivo;
                existente.Observacion = dto.Observacion;
                _datInforme.Update(existente);
                await _datInforme.SaveChangesAsync();

                _logger.LogInformation("Informe actualizado (upsert): {Id}", existente.IdInformeMensual);
                return ApiResponse<Guid>.Ok(existente.IdInformeMensual, "Informe actualizado.");
            }

            var informe = new InformeMensual
            {
                IdInformeMensual = Guid.NewGuid(),
                IdPublicador = dto.IdPublicador,
                Ano = dto.Ano,
                Mes = dto.Mes,
                Participo = dto.Participo,
                CursosBiblicos = cursos,
                Horas = horas,
                Tipo = tipoFinal,
                Inactivo = dto.Inactivo,
                Observacion = dto.Observacion
            };

            await _datInforme.AddAsync(informe);
            await _datInforme.SaveChangesAsync();

            _logger.LogInformation("Informe registrado: {Id}", informe.IdInformeMensual);
            return ApiResponse<Guid>.Ok(informe.IdInformeMensual, "Informe registrado.", 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar informe.");
            return ApiResponse<Guid>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<string>> ActualizarAsync(ActualizarInformeDto dto)
    {
        try
        {
            var informe = await _datInforme.GetByIdAsync(dto.IdInformeMensual);
            if (informe == null)
                return ApiResponse<string>.NotFound(
                    $"Informe con Id ({dto.IdInformeMensual}) no encontrado.",
                    ErrorCatalog.EntidadNoEncontrada);

            var tipoFinal = dto.TipoInformativo ?? informe.Tipo;

            var errores = ValidarInforme(dto.Horas, dto.CursosBiblicos, informe.Ano, informe.Mes);
            if (errores.Count > 0)
                return ApiResponse<string>.Fail("Errores de validación.", ErrorCatalog.ValidacionFallida, 400, errores);

            var (horas, cursos) = LimpiarCamposPorTipo(tipoFinal, dto.Inactivo, dto.Participo, dto.Horas, dto.CursosBiblicos);

            informe.Participo = dto.Participo;
            informe.CursosBiblicos = cursos;
            informe.Horas = horas;
            informe.Tipo = tipoFinal;
            informe.Inactivo = dto.Inactivo;
            informe.Observacion = dto.Observacion;

            _datInforme.Update(informe);
            await _datInforme.SaveChangesAsync();

            _logger.LogInformation("Informe actualizado: {Id}", dto.IdInformeMensual);
            return ApiResponse<string>.Ok("Actualizado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar informe: {Id}.", dto.IdInformeMensual);
            return ApiResponse<string>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<string>> EliminarAsync(Guid id)
    {
        try
        {
            var informe = await _datInforme.GetByIdAsync(id);
            if (informe == null)
                return ApiResponse<string>.NotFound(
                    $"Informe con Id ({id}) no encontrado.",
                    ErrorCatalog.EntidadNoEncontrada);

            _datInforme.Delete(informe);
            await _datInforme.SaveChangesAsync();

            _logger.LogInformation("Informe eliminado: {Id}", id);
            return ApiResponse<string>.Ok("Eliminado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar informe: {Id}.", id);
            return ApiResponse<string>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<ResultadoImportacionDto>> ImportarExcelAsync(ImportarInformesDto meta, Stream archivo)
    {
        try
        {
            var grupo = await _datGrupo.GetByIdAsync(meta.IdGrupo);
            if (grupo == null)
                return ApiResponse<ResultadoImportacionDto>.NotFound(
                    $"Grupo con Id ({meta.IdGrupo}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            using var workbook = new XLWorkbook(archivo);

            var filas = LeerExcel(workbook);
            var resultado = new ResultadoImportacionDto();
            var todosPublicadores = await _datPublicador.GetAllAsync();
            var filasValidas = new List<(Publicador publicador, InformeExcelRowDto fila, TipoPublicador tipo)>();

            // ── FASE 1: Validar todo ─────────────────────────────────────────
            foreach (var fila in filas)
            {
                Publicador? publicador = null;

                // Buscar por nombre
                if (!string.IsNullOrWhiteSpace(fila.Nombre))
                {
                    publicador = todosPublicadores
                        .FirstOrDefault(p => NormalizarTexto(p.NombreCompleto) == NormalizarTexto(fila.Nombre));
                }

                // Si no existe por nombre → ignorar la fila
                if (publicador == null)
                    continue;

                if (!Enum.TryParse<TipoPublicador>(fila.Tipo, true, out var tipoParseado))
                {
                    resultado.Errores.Add($"Tipo '{fila.Tipo}' no válido para '{fila.Nombre}'.");
                    resultado.Fallidos++;
                    continue;
                }

                var errores = ValidarInforme(fila.Horas, fila.Cursos, meta.Ano, meta.Mes);
                if (errores.Count > 0)
                {
                    resultado.Errores.AddRange(errores.Select(e => $"{fila.Nombre}: {e}"));
                    resultado.Fallidos++;
                    continue;
                }

                filasValidas.Add((publicador, fila, tipoParseado));
                resultado.Exitosos++;
            }

            // ── FASE 2: Si hay errores NO guardar nada ───────────────────────
            if (resultado.Fallidos > 0)
            {
                resultado.Exitosos = 0;
                return ApiResponse<ResultadoImportacionDto>.Ok(
                    resultado,
                    $"Importación cancelada: {resultado.Fallidos} error(es). Corrija el archivo e intente de nuevo.");
            }

            // ── FASE 3: Guardar informes ─────────────────────────────────────
            var nuevos = new List<InformeMensual>();

            foreach (var (publicador, fila, tipo) in filasValidas)
            {
                var (horas, cursos) = LimpiarCamposPorTipo(tipo, fila.Inactivo, fila.Participo, fila.Horas, fila.Cursos);
                var existente = await _datInforme.GetByPublicadorMesAsync(publicador.IdPublicador, meta.Ano, meta.Mes);

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
                        Ano = meta.Ano,
                        Mes = meta.Mes,
                        Participo = fila.Participo,
                        CursosBiblicos = cursos,
                        Horas = horas,
                        Tipo = tipo,
                        Inactivo = fila.Inactivo,
                        Observacion = fila.Observacion
                    });
                }
            }

            if (nuevos.Any())
                await _datInforme.AddRangeAsync(nuevos);

            await _datInforme.SaveChangesAsync();

            // ── FASE 5: Actualizar tipo del publicador según el informe más reciente ──
            var hoy = DateTime.UtcNow;
            var valorFechaHoy = hoy.Year * 12 + hoy.Month;
            var publicadoresAfectados = filasValidas
                .Select(f => f.publicador)
                .DistinctBy(p => p.IdPublicador);

            foreach (var pub in publicadoresAfectados)
            {
                var informesPub = await _datInforme.GetByPublicadorAsync(pub.IdPublicador);
                var masReciente = informesPub
                    .Where(i => i.Ano * 12 + i.Mes <= valorFechaHoy)
                    .OrderByDescending(i => i.Ano * 12 + i.Mes)
                    .FirstOrDefault();

                if (masReciente != null && pub.Tipo != masReciente.Tipo)
                {
                    pub.Tipo = masReciente.Tipo;
                    _datPublicador.Update(pub);
                }
            }

            await _datPublicador.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("Importación Excel: {Exitosos} exitosos.", resultado.Exitosos);
            return ApiResponse<ResultadoImportacionDto>.Ok(resultado, "Importación completada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al importar Excel.");
            return ApiResponse<ResultadoImportacionDto>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<byte[]>> DescargarTemplateAsync(Guid idGrupo)
    {
        try
        {
            var grupos = await _datGrupo.GetConMiembrosAsync();
            var grupo = grupos.FirstOrDefault(g => g.IdGrupo == idGrupo);

            if (grupo == null)
                return ApiResponse<byte[]>.NotFound(
                    $"Grupo con Id ({idGrupo}) no encontrado.",
                    ErrorCatalog.EntidadNoEncontrada);

            var bytes = GenerarTemplateExcel(grupo);
            return ApiResponse<byte[]>.Ok(bytes, "Template generado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar template Excel.");
            return ApiResponse<byte[]>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    // ── Helpers privados ─────────────────────────────────────────────────────

    private static List<string> ValidarInforme(
        int? horas, int cursos, int ano, int mes)
    {
        var errores = new List<string>();

        if (ano < 2000 || ano > 2100)
            errores.Add("El año debe estar entre 2000 y 2100.");

        if (mes < 1 || mes > 12)
            errores.Add("El mes debe estar entre 1 y 12.");

        if (cursos < 0)
            errores.Add("Los cursos bíblicos no pueden ser negativos.");

        if (horas.HasValue && horas < 0)
            errores.Add("Las horas no pueden ser negativas.");

        return errores;
    }

    private static (int? horas, int cursos) LimpiarCamposPorTipo(
        TipoPublicador tipo, bool inactivo, bool participo, int? horas, int cursos)
    {
        if (inactivo)
            return (null, 0);

        if (!participo)
            return (null, 0);

        if (tipo == TipoPublicador.Publicador || tipo == TipoPublicador.NoBautizado)
            return (null, cursos);

        return (horas, cursos);
    }

    private static List<InformeExcelRowDto> LeerExcel(XLWorkbook workbook)
    {
        var resultado = new List<InformeExcelRowDto>();
        var ws = workbook.Worksheets.First();

        foreach (var row in ws.RowsUsed().Skip(1))
        {
            if (string.IsNullOrWhiteSpace(row.Cell(1).GetString()))
                continue;

            resultado.Add(new InformeExcelRowDto
            {
                Nombre = row.Cell(1).GetString().Trim(),
                Tipo = row.Cell(2).GetString().Trim(),
                Participo = ParseBool(row.Cell(3).GetString()),
                Horas = row.Cell(4).IsEmpty() ? null : (int?)Convert.ToInt32(row.Cell(4).GetDouble()),
                Cursos = row.Cell(5).IsEmpty() ? 0 : Convert.ToInt32(row.Cell(5).GetDouble()),
                Inactivo = ParseBool(row.Cell(6).GetString()),
                Observacion = row.Cell(7).IsEmpty() ? null : row.Cell(7).GetString().Trim(),
                IdPublicador = row.Cell(8).GetString().Trim()
            });
        }

        return resultado;
    }

    private static bool ParseBool(string valor)
    {
        var v = valor.Trim().ToLower();
        return v is "sí" or "si" or "1" or "true" or "yes";
    }

    private static byte[] GenerarTemplateExcel(Grupo grupo)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Informes");

        // ── Encabezados ───────────────────────────────────────────────────────
        ws.Cell(1, 1).Value = "Nombre";
        ws.Cell(1, 2).Value = "Tipo";
        ws.Cell(1, 3).Value = "Participó";
        ws.Cell(1, 4).Value = "Horas";
        ws.Cell(1, 5).Value = "Cursos";
        ws.Cell(1, 6).Value = "Inactivo";
        ws.Cell(1, 7).Value = "Observación";
        ws.Cell(1, 8).Value = "IdPublicador"; // ← oculta

        var header = ws.Range(1, 1, 1, 7);
        header.Style.Font.Bold = true;
        header.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
        header.Style.Font.FontColor = XLColor.White;
        header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // ── Publicadores del grupo ────────────────────────────────────────────
        var orden = new Dictionary<TipoPublicador, int>
    {
        { TipoPublicador.Publicador,        0 },
        { TipoPublicador.PrecursorAuxiliar, 1 },
        { TipoPublicador.PrecursorRegular,  2 },
        { TipoPublicador.NoBautizado,       3 }
    };

        var publicadores = grupo.Publicadores
            .OrderBy(p => orden.GetValueOrDefault(p.Tipo, 99))
            .ThenBy(p => p.NombreCompleto)
            .ToList();

        for (int i = 0; i < publicadores.Count; i++)
        {
            int fila = i + 2;
            var pub = publicadores[i];
            ws.Cell(fila, 1).SetValue(pub.NombreCompleto);
            ws.Cell(fila, 2).SetValue(pub.Tipo.ToString());
            ws.Cell(fila, 3).SetValue("Sí");
            ws.Cell(fila, 6).SetValue("No");
            ws.Cell(fila, 8).SetValue(pub.IdPublicador.ToString()); // ← ID oculto
        }

        // ── Validaciones ──────────────────────────────────────────────────────
        int ultimaFila = publicadores.Count + 1;

        var tiposValidos = string.Join(",", Enum.GetNames(typeof(TipoPublicador)));
        var tipoValidation = ws.Range(2, 2, ultimaFila, 2).SetDataValidation();
        tipoValidation.List($"\"{tiposValidos}\"");
        tipoValidation.ShowErrorMessage = true;
        tipoValidation.ErrorTitle = "Tipo inválido";
        tipoValidation.ErrorMessage = "Seleccione un tipo válido de la lista.";

        foreach (int col in new[] { 3, 6 })
        {
            var v = ws.Range(2, col, ultimaFila, col).SetDataValidation();
            v.List("\"Sí,No\"");
            v.ShowErrorMessage = true;
            v.ErrorTitle = "Valor inválido";
            v.ErrorMessage = "Seleccione Sí o No.";
        }

        var horasV = ws.Range(2, 4, ultimaFila, 4).SetDataValidation();
        horasV.WholeNumber.Between(0, 999);
        horasV.ShowErrorMessage = true;
        horasV.ErrorTitle = "Valor inválido";
        horasV.ErrorMessage = "Las horas deben ser un número entre 0 y 999.";

        var cursosV = ws.Range(2, 5, ultimaFila, 5).SetDataValidation();
        cursosV.WholeNumber.Between(0, 999);
        cursosV.ShowErrorMessage = true;
        cursosV.ErrorTitle = "Valor inválido";
        cursosV.ErrorMessage = "Los cursos deben ser un número entre 0 y 999.";

        // ── Protección y ancho ────────────────────────────────────────────────
        ws.Column(1).Style.Protection.Locked = true;
        ws.Column(8).Style.Protection.Locked = true; // ← proteger ID
        ws.Columns(2, 7).Style.Protection.Locked = false;

        ws.Column(1).Width = 35;
        ws.Column(2).Width = 20;
        ws.Column(3).Width = 12;
        ws.Column(4).Width = 10;
        ws.Column(5).Width = 10;
        ws.Column(6).Width = 12;
        ws.Column(7).Width = 30;
        ws.Column(8).Hide(); // ← ocultar

        ws.SheetView.FreezeRows(1);

        // ── Hoja Meta oculta ──────────────────────────────────────────────────
        var wsMeta = workbook.Worksheets.Add("Meta");
        wsMeta.Cell(1, 1).Value = "Grupo";
        wsMeta.Cell(1, 2).Value = grupo.Nombre;
        wsMeta.Cell(2, 1).Value = "IdGrupo";
        wsMeta.Cell(2, 2).SetValue(grupo.IdGrupo.ToString());
        wsMeta.Visibility = XLWorksheetVisibility.Hidden;

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }
    private static string NormalizarTexto(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

        var normalizado = texto.Trim().ToLower();
        // Eliminar tildes
        normalizado = string.Concat(
            normalizado.Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                    != System.Globalization.UnicodeCategory.NonSpacingMark));

        // Eliminar espacios dobles
        while (normalizado.Contains("  "))
            normalizado = normalizado.Replace("  ", " ");

        return normalizado;
    }
}