namespace ControlInformes.Business.DTOs;

public class DashboardDto
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public KpisDto Kpis { get; set; } = new();
    public TiposPublicadorDto TiposPublicador { get; set; } = new();
    public List<DistribucionDto> Distribucion { get; set; } = [];
    public List<HistorialMesDto> HistorialSemestral { get; set; } = [];
    public VariacionesDto Variaciones { get; set; } = new();
}

public class KpisDto
{
    public int TotalPublicadoresActivos { get; set; }
    public int InformesRecibidos { get; set; }
    public int TotalCursosBiblicos { get; set; }
    public int TotalHorasPrecursores { get; set; }
    public double PromedioAsistencia { get; set; }
}

public class TiposPublicadorDto
{
    public int Publicadores { get; set; }
    public int PrecursoresAuxiliares { get; set; }
    public int PrecursoresRegulares { get; set; }
}

public class DistribucionDto
{
    public string Tipo { get; set; } = string.Empty;
    public int Informes { get; set; }
    public int Cursos { get; set; }
    public int Horas { get; set; }
}

public class HistorialMesDto
{
    public string Mes { get; set; } = string.Empty;
    public int Ano { get; set; }
    public int PublicadoresActivos { get; set; }
    public int Informes { get; set; }
    public int Cursos { get; set; }
    public int Horas { get; set; }
}

public class VariacionesDto
{
    public double CambioInformes { get; set; }
    public double CambioCursos { get; set; }
    public double CambioHoras { get; set; }
    public double CambioAsistencia { get; set; }
}