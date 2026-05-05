using AutoMapper;
using ControlInformes.Business.DTOs;
using ControlInformes.Business.Interfaces;
using ControlInformes.Data.Interfaces;
using ControlInformes.Domain.Entities;
using ControlInformes.Domain.Enums;
using ControlInformes.Utils;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using Microsoft.Extensions.Logging;

namespace ControlInformes.Business.Implementations;

public class BusPublicador : IBusPublicador
{
    private readonly IDatPublicador _datPublicador;
    private readonly IDatInformeMensual _datInforme;
    private readonly IMapper _mapper;
    private readonly ILogger<BusPublicador> _logger;

    private static readonly string[] _nombresMeses =
    [
        "", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
    ];

    public BusPublicador(
        IDatPublicador datPublicador,
        IDatInformeMensual datInforme,
        IMapper mapper,
        ILogger<BusPublicador> logger)
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
            return ApiResponse<List<PublicadorDto>>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<PublicadorDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var publicador = await _datPublicador.GetByIdAsync(id);
            if (publicador == null)
                return ApiResponse<PublicadorDto>.NotFound(
                    $"Publicador con Id ({id}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            var result = _mapper.Map<PublicadorDto>(publicador);
            return ApiResponse<PublicadorDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener publicador por Id: {Id}.", id);
            return ApiResponse<PublicadorDto>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<Guid>> CrearAsync(CrearPublicadorDto dto)
    {
        try
        {
            var erroresRol = ValidarRol(dto.Tipo, dto.Rol);
            if (erroresRol.Count > 0)
                return ApiResponse<Guid>.Fail(
                    "Errores de validación.", ErrorCatalog.ValidacionFallida, 400, erroresRol);

            var publicador = _mapper.Map<Publicador>(dto);
            publicador.IdPublicador = Guid.NewGuid();
            publicador.Activo = true;
            publicador.FechaCreacion = DateTime.Now;

            await _datPublicador.AddAsync(publicador);
            await _datPublicador.SaveChangesAsync();

            _logger.LogInformation("Publicador creado: {Id} - {Nombre}",
                publicador.IdPublicador, publicador.NombreCompleto);

            return ApiResponse<Guid>.Ok(publicador.IdPublicador, "Publicador creado.", 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear publicador.");
            return ApiResponse<Guid>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<string>> ActualizarAsync(ActualizarPublicadorDto dto)
    {
        try
        {
            var publicador = await _datPublicador.GetByIdAsync(dto.IdPublicador);
            if (publicador == null)
                return ApiResponse<string>.NotFound(
                    $"Publicador con Id ({dto.IdPublicador}) no encontrado.",
                    ErrorCatalog.EntidadNoEncontrada);

            var erroresRol = ValidarRol(dto.Tipo, dto.Rol);
            if (erroresRol.Count > 0)
                return ApiResponse<string>.Fail(
                    "Errores de validación.", ErrorCatalog.ValidacionFallida, 400, erroresRol);

            publicador.NombreCompleto = dto.NombreCompleto;
            publicador.FechaNacimiento = dto.FechaNacimiento;
            publicador.FechaBautismo = dto.FechaBautismo;
            publicador.Genero = dto.Genero;
            publicador.CondicionEspiritual = dto.CondicionEspiritual;
            publicador.Tipo = dto.Tipo;
            publicador.Rol = dto.Rol;
            publicador.IdGrupo = dto.IdGrupo;
            publicador.Inactivo = dto.Inactivo;
            publicador.Activo = true;

            _datPublicador.Update(publicador);
            await _datPublicador.SaveChangesAsync();

            _logger.LogInformation("Publicador actualizado: {Id}", dto.IdPublicador);
            return ApiResponse<string>.Ok("Actualizado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar publicador: {Id}.", dto.IdPublicador);
            return ApiResponse<string>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<string>> EliminarAsync(Guid id)
    {
        try
        {
            var publicador = await _datPublicador.GetByIdAsync(id);
            if (publicador == null)
                return ApiResponse<string>.NotFound(
                    $"Publicador con Id ({id}) no encontrado.", ErrorCatalog.EntidadNoEncontrada);

            _datPublicador.Delete(publicador);
            await _datPublicador.SaveChangesAsync();

            _logger.LogInformation("Publicador eliminado: {Id}", id);
            return ApiResponse<string>.Ok("Eliminado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar publicador: {Id}.", id);
            return ApiResponse<string>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<TarjetaPublicadorDto>> GetTarjetaAsync(Guid idPublicador, int? anoServicio)
    {
        try
        {
            var publicador = await _datPublicador.GetByIdAsync(idPublicador);
            if (publicador == null)
                return ApiResponse<TarjetaPublicadorDto>.NotFound(
                    $"Publicador con Id ({idPublicador}) no encontrado.",
                    ErrorCatalog.EntidadNoEncontrada);

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
                FechaNacimiento = publicador.FechaNacimiento,
                FechaBautismo = publicador.FechaBautismo,
                Genero = publicador.Genero,
                GeneroDescripcion = publicador.Genero.ToString(),
                CondicionEspiritual = publicador.CondicionEspiritual,
                CondicionEspiritualDescripcion = publicador.CondicionEspiritual.ToString(),
                Tipo = publicador.Tipo,
                TipoDescripcion = publicador.Tipo.ToString(),
                Rol = publicador.Rol,
                RolDescripcion = publicador.Rol.ToString(),
                NombreGrupo = publicador.Grupo?.Nombre ?? string.Empty,
                AnoServicioInicio = anoInicio,
                AnoServicioFin = anoFin,
                Meses = meses
            };

            return ApiResponse<TarjetaPublicadorDto>.Ok(tarjeta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tarjeta del publicador: {Id}.", idPublicador);
            return ApiResponse<TarjetaPublicadorDto>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
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
            return ApiResponse<List<PublicadorDto>>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    public async Task<ApiResponse<PagedResult<PublicadorGrupoDto>>> GetListadoPaginadoAsync(FiltroPublicadorGrupoDto filtro)
    {
        try
        {
            var (items, total) = await _datPublicador.GetPaginadoConGrupoAsync(
                filtro.IdGrupo,
                filtro.IdPublicador,
                filtro.NombreCompleto,
                filtro.Tipo,
                filtro.Inactivo,
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
                Inactivo = p.Inactivo
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

    public async Task<ApiResponse<byte[]>> DescargarTarjetaPdfAsync(Guid idPublicador, int? anoServicio)
    {
        try
        {
            if (!File.Exists(Path.Combine(AppContext.BaseDirectory, "Template", "Template_Tarjeta_Publicador.pdf")))
                return ApiResponse<byte[]>.Error(
                    "Template de tarjeta no encontrado.", ErrorCatalog.ArchivoInvalido);

            var tarjetaResponse = await GetTarjetaAsync(idPublicador, anoServicio);
            if (tarjetaResponse.HasError)
                return ApiResponse<byte[]>.Error(tarjetaResponse.Mensaje, tarjetaResponse.CodigoError);

            var bytes = GenerarPdfTarjeta(tarjetaResponse.Result!);
            return ApiResponse<byte[]>.Ok(bytes, "PDF generado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar PDF de tarjeta: {Id}.", idPublicador);
            return ApiResponse<byte[]>.Error(
                ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno), ErrorCatalog.ErrorInterno);
        }
    }

    // ── Helpers privados ─────────────────────────────────────────────────────

    private static List<string> ValidarRol(TipoPublicador tipo, RolCongregacion rol)
    {
        var errores = new List<string>();

        if (tipo == TipoPublicador.NoBautizado && rol != RolCongregacion.Ninguno)
            errores.Add("Un publicador no bautizado no puede tener rol de Anciano o Siervo Ministerial.");

        return errores;
    }

    private static TarjetaMesDto MapMes(int ano, int mes, InformeMensual? informe)
    {
        return new TarjetaMesDto
        {
            Ano = ano,
            Mes = mes,
            NombreMes = _nombresMeses[mes],
            Participo = informe?.Participo ?? false,
            CursosBiblicos = informe?.CursosBiblicos ?? 0,
            Horas = informe?.Horas,
            PrecursorAuxiliar = informe?.Tipo == TipoPublicador.PrecursorAuxiliar,
            Notas = informe == null ? "Sin informe" : null
        };
    }

    private static byte[] GenerarPdfTarjeta(TarjetaPublicadorDto tarjeta)
    {
        using var ms = new MemoryStream();
        using var reader = new PdfReader(
            Path.Combine(AppContext.BaseDirectory, "Template", "Template_Tarjeta_Publicador.pdf"));
        using var writer = new PdfWriter(ms);
        using var pdfDoc = new PdfDocument(reader, writer);
        var form = PdfAcroForm.GetAcroForm(pdfDoc, false);

        // ── Datos personales ──────────────────────────────────────────────
        SetCampo(form, "900_1_Text_SanSerif", tarjeta.NombreCompleto);
        SetCampo(form, "900_2_Text_SanSerif", tarjeta.FechaNacimiento?.ToString("dd/MM/yyyy") ?? string.Empty);
        SetCampo(form, "900_5_Text_SanSerif", tarjeta.FechaBautismo?.ToString("dd/MM/yyyy") ?? string.Empty);
        SetCampo(form, "900_13_Text_C_SanSerif", tarjeta.AnoServicioInicio.ToString());

        // ── Género ────────────────────────────────────────────────────────
        SetCheckbox(form, "900_3_CheckBox", tarjeta.Genero == Genero.Hombre);
        SetCheckbox(form, "900_4_CheckBox", tarjeta.Genero == Genero.Mujer);

        // ── Condición espiritual ──────────────────────────────────────────
        SetCheckbox(form, "900_6_CheckBox", tarjeta.CondicionEspiritual == CondicionEspiritual.OtrasOvejas);
        SetCheckbox(form, "900_7_CheckBox", tarjeta.CondicionEspiritual == CondicionEspiritual.Ungido);

        // ── Rol en la congregación ────────────────────────────────────────
        SetCheckbox(form, "900_8_CheckBox", tarjeta.Rol == RolCongregacion.Anciano);
        SetCheckbox(form, "900_9_CheckBox", tarjeta.Rol == RolCongregacion.SiervoMinisterial);
        SetCheckbox(form, "900_10_CheckBox", tarjeta.Tipo == TipoPublicador.PrecursorRegular);
        SetCheckbox(form, "900_11_CheckBox", false); // Precursor especial — no aplica
        SetCheckbox(form, "900_12_CheckBox", false); // Misionero — no aplica

        // ── Datos por mes ─────────────────────────────────────────────────
        // Sep=20, Oct=21, Nov=22, Dic=23, Ene=24, Feb=25
        // Mar=26, Abr=27, May=28, Jun=29, Jul=30, Ago=31
        for (int i = 0; i < tarjeta.Meses.Count && i < 12; i++)
        {
            var mes = tarjeta.Meses[i];
            int numero = 20 + i;

            SetCheckbox(form, $"901_{numero}_CheckBox", mes.Participo);
            SetCampo(form, $"902_{numero}_Text_C_SanSerif", mes.Participo && mes.CursosBiblicos > 0
                ? mes.CursosBiblicos.ToString()
                : string.Empty);
            SetCheckbox(form, $"903_{numero}_CheckBox", mes.PrecursorAuxiliar);
            SetCampo(form, $"904_{numero}_S21_Value", mes.Horas.HasValue
                ? mes.Horas.Value.ToString()
                : string.Empty);
            SetCampo(form, $"905_{numero}_Text_SanSerif", mes.Notas ?? string.Empty);
        }

        // ── Totales ───────────────────────────────────────────────────────
        int totalHoras = tarjeta.Meses.Sum(m => m.Horas ?? 0);
        int totalCursos = tarjeta.Meses.Sum(m => m.CursosBiblicos);

        SetCampo(form, "904_32_S21_Value", totalHoras > 0 ? totalHoras.ToString() : string.Empty);
        SetCampo(form, "905_32_Text_SanSerif", totalCursos > 0 ? totalCursos.ToString() : string.Empty);

        // Aplanar: PDF no editable al descargar
        form.FlattenFields();

        pdfDoc.Close();
        return ms.ToArray();
    }

    private static void SetCampo(PdfAcroForm form, string nombre, string valor)
    {
        var campo = form.GetField(nombre);
        if (campo != null)
            campo.SetValue(valor);
    }

    private static void SetCheckbox(PdfAcroForm form, string nombre, bool marcado)
    {
        var campo = form.GetField(nombre);
        if (campo == null) return;
        campo.SetValue(marcado ? "Yes" : "Off");
    }
}