namespace ControlInformes.Business.DTOs;

public class DashboardDto
{
    // ── Contexto del filtro ───────────────────────────────────────────────
    public int AnoServicioInicio { get; set; }  // ← nuevo
    public int AnoServicioFin { get; set; }      // ← nuevo
    public int? MesFiltrado { get; set; }        // ← nuevo (null = todos)

    // ── Cards (se alimentan del filtro) ──────────────────────────────────
    public int TotalPublicadoresActivos { get; set; }
    public int TotalInactivos { get; set; }
    public CardInformesDto Publicadores { get; set; } = new();
    public CardInformesDto PrecursoresAuxiliares { get; set; } = new();
    public CardInformesDto PrecursoresRegulares { get; set; } = new();
    public CardReunionDto ReunionesPublicas { get; set; } = new();
    public CardReunionDto ReunionesServicio { get; set; } = new();

    // ── Gráficas (siempre 12 meses Sep→Ago) ──────────────────────────────
    public List<DistribucionTipoDto> DistribucionPorTipo { get; set; } = new();
    public List<HistorialMesDto> Historial12Meses { get; set; } = new();  // ← renombrado
}

public class CardInformesDto
{
    public int CantidadInformes { get; set; }
    public double Variacion { get; set; }
}

public class CardReunionDto
{
    public int CantidadReuniones { get; set; }
    public double Promedio { get; set; }
    public double Variacion { get; set; }
}

public class DistribucionTipoDto
{
    public string Tipo { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class HistorialMesDto
{
    public int Ano { get; set; }
    public int NumeroMes { get; set; }          // ← nuevo para identificar el mes
    public string Mes { get; set; } = string.Empty;
    public int Publicadores { get; set; }
    public int PrecursoresAuxiliares { get; set; }
    public int PrecursoresRegulares { get; set; }
    public bool EsMesFiltrado { get; set; }     // ← nuevo para resaltar en gráfica
}