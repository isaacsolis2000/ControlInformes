namespace ControlInformes.Business.DTOs;

public class DashboardDto
{
    public int Ano { get; set; }
    public int Mes { get; set; }

    // Cards
    public int TotalPublicadoresActivos { get; set; }
    public int TotalInactivos { get; set; }

    public CardInformesDto Publicadores { get; set; } = new();
    public CardInformesDto PrecursoresAuxiliares { get; set; } = new();
    public CardInformesDto PrecursoresRegulares { get; set; } = new();

    public CardReunionDto ReunionesPublicas { get; set; } = new();
    public CardReunionDto ReunionesServicio { get; set; } = new();

    // Gráfica de pastel: distribución por tipo
    public List<DistribucionTipoDto> DistribucionPorTipo { get; set; } = new();

    // Gráfica de barras: últimos 6 meses por tipo
    public List<HistorialMesDto> HistorialSemestral { get; set; } = new();
}

public class CardInformesDto
{
    public int CantidadInformes { get; set; }
    public double Variacion { get; set; } // % vs mes anterior
}

public class CardReunionDto
{
    public int CantidadReuniones { get; set; }
    public double Promedio { get; set; }
    public double Variacion { get; set; } // % vs mes anterior
}

public class DistribucionTipoDto
{
    public string Tipo { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class HistorialMesDto
{
    public int Ano { get; set; }
    public string Mes { get; set; } = string.Empty;
    public int Publicadores { get; set; }
    public int PrecursoresAuxiliares { get; set; }
    public int PrecursoresRegulares { get; set; }
}