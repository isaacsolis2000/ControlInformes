using ClosedXML.Excel;
using ControlInformes.Application.Features.Excel;

namespace ControlInformes.Infrastructure.Services;

public class ExcelService : IExcelService
{
    public List<ExcelInformeRow> LeerInformes(Stream stream)
    {
        var resultado = new List<ExcelInformeRow>();

        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();

        var rows = worksheet.RowsUsed().Skip(1);

        foreach (var row in rows)
        {
            var fila = new ExcelInformeRow
            {
                Nombre = row.Cell(1).GetString().Trim(),
                Tipo = row.Cell(2).GetString().Trim(),
                Participo = row.Cell(3).GetString().Trim().Equals("Sí", StringComparison.OrdinalIgnoreCase)
                            || row.Cell(3).GetString().Trim().Equals("Si", StringComparison.OrdinalIgnoreCase)
                            || row.Cell(3).GetString().Trim() == "1"
                            || row.Cell(3).GetString().Trim().Equals("true", StringComparison.OrdinalIgnoreCase),
                Horas = row.Cell(4).IsEmpty() ? null : (int?)row.Cell(4).GetDouble(),
                Cursos = row.Cell(5).IsEmpty() ? 0 : (int)row.Cell(5).GetDouble()
            };

            resultado.Add(fila);
        }

        return resultado;
    }
}
