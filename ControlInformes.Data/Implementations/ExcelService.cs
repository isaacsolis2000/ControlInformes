using ClosedXML.Excel;
using ControlInformes.Data.Interfaces;
using ControlInformes.Domain.Enums;

namespace ControlInformes.Data.Implementations;

public class ExcelService : IExcelService
{
    public List<ExcelInformeRow> LeerInformes(Stream stream)
    {
        var resultado = new List<ExcelInformeRow>();

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();

        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            // Saltar filas completamente vacías
            if (string.IsNullOrWhiteSpace(row.Cell(1).GetString()))
                continue;

            resultado.Add(new ExcelInformeRow
            {
                Nombre = row.Cell(1).GetString().Trim(),
                Tipo = row.Cell(2).GetString().Trim(),
                Participo = ParseBool(row.Cell(3).GetString()),
                Horas = row.Cell(4).IsEmpty() ? null : (int?)row.Cell(4).GetDouble(),
                Cursos = row.Cell(5).IsEmpty() ? 0 : (int)row.Cell(5).GetDouble(),
                Inactivo = ParseBool(row.Cell(6).GetString()),       // ← nuevo
                Observacion = row.Cell(7).IsEmpty()                     // ← nuevo
                    ? null
                    : row.Cell(7).GetString().Trim()
            });
        }

        return resultado;
    }

    public byte[] GenerarTemplate(List<string> nombresPublicadores)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Informes");

        // ── Encabezados ───────────────────────────────────────────────────────
        var encabezados = new[] { "Nombre", "Tipo", "Participó", "Horas", "Cursos", "Inactivo", "Observación" };
        for (int i = 0; i < encabezados.Length; i++)
            ws.Cell(1, i + 1).Value = encabezados[i];

        // Estilo encabezados
        var headerRange = ws.Range(1, 1, 1, encabezados.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // ── Validación desplegable Tipo ───────────────────────────────────────
        var tiposValidos = string.Join(",", Enum.GetNames(typeof(TipoPublicador)));
        var tipoValidation = ws.Range(2, 2, 1000, 2).SetDataValidation();
        tipoValidation.List($"\"{tiposValidos}\"");
        tipoValidation.ErrorMessage = "Seleccione un tipo válido.";
        tipoValidation.ShowErrorMessage = true;

        // ── Validación desplegable Participó e Inactivo ───────────────────────
        foreach (int col in new[] { 3, 6 })
        {
            var validation = ws.Range(2, col, 1000, col).SetDataValidation();
            validation.List("\"Sí,No\"");
            validation.ErrorMessage = "Seleccione Sí o No.";
            validation.ShowErrorMessage = true;
        }

        // ── Llenar publicadores ───────────────────────────────────────────────
        for (int i = 0; i < nombresPublicadores.Count; i++)
        {
            int fila = i + 2;
            ws.Cell(fila, 1).Value = nombresPublicadores[i];
            ws.Cell(fila, 3).Value = "No";   // Participó default
            ws.Cell(fila, 6).Value = "No";   // Inactivo default
        }

        // ── Proteger columna Nombre ───────────────────────────────────────────
        ws.Column(1).Style.Protection.Locked = true;

        ws.Columns().AdjustToContents();
        ws.SheetView.FreezeRows(1); // Fijar encabezado

        using var memStream = new MemoryStream();
        workbook.SaveAs(memStream);
        return memStream.ToArray();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static bool ParseBool(string valor)
    {
        var v = valor.Trim().ToLower();
        return v is "sí" or "si" or "1" or "true" or "yes";
    }
}