using ControlInformes.Domain.Enums;

namespace ControlInformes.Business.DTOs;

// Listado detalle
public class InformeMensualDto
{
    public Guid IdInformeMensual { get; set; }
    public Guid IdPublicador { get; set; }
    public string NombrePublicador { get; set; } = string.Empty;
    public Guid? IdGrupo { get; set; }
    public string NombreGrupo { get; set; } = string.Empty;
    public int Ano { get; set; }
    public int Mes { get; set; }
    public bool Participo { get; set; }
    public int CursosBiblicos { get; set; }
    public int? Horas { get; set; }
    public TipoPublicador Tipo { get; set; }
    public string TipoDescripcion { get; set; } = string.Empty;
    public bool Inactivo { get; set; }
    public string? Observacion { get; set; }
}

// Crear informe
public class RegistrarInformeDto
{
    public Guid IdPublicador { get; set; }
    public int Ano { get; set; }
    public int Mes { get; set; }
    public bool Participo { get; set; }
    public int CursosBiblicos { get; set; }
    public int? Horas { get; set; }
    public TipoPublicador? TipoInformativo { get; set; } // Override del tipo solo para ese mes
    public bool Inactivo { get; set; }
    public string? Observacion { get; set; }
}

// Actualizar informe
public class ActualizarInformeDto
{
    public Guid IdInformeMensual { get; set; }
    public bool Participo { get; set; }
    public int CursosBiblicos { get; set; }
    public int? Horas { get; set; }
    public TipoPublicador? TipoInformativo { get; set; }
    public bool Inactivo { get; set; }
    public string? Observacion { get; set; }
}

// Filtros detalle
public class FiltroInformeDto
{
    public int? Ano { get; set; }
    public int? Mes { get; set; }
    public Guid? IdGrupo { get; set; }
    public Guid? IdPublicador { get; set; }
    public TipoPublicador? Tipo { get; set; }
    public bool? Inactivo { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanoPagina { get; set; } = 50;
}

// Tab Total
public class TotalInformeDto
{
    public int Ano { get; set; }
    public int Mes { get; set; }

    // Informes generales
    public int TotalPublicadores { get; set; }
    public double PromedioAsistenciaReuniones { get; set; }

    // Publicadores
    public ResumenTipoDto Publicadores { get; set; } = new();

    // Precursores auxiliares
    public ResumenPrecursorDto PrecursoresAuxiliares { get; set; } = new();

    // Precursores regulares
    public ResumenPrecursorDto PrecursoresRegulares { get; set; } = new();

    // Pendientes
    public PendientesDto Pendientes { get; set; } = new();
}

public class ResumenTipoDto
{
    public int CantidadInformes { get; set; }
    public int CursosBiblicos { get; set; }
}

public class ResumenPrecursorDto
{
    public int CantidadInformes { get; set; }
    public int Horas { get; set; }
    public int CursosBiblicos { get; set; }
}

public class PendientesDto
{
    public bool ReunionesRegistradas { get; set; }
    public List<string> GruposSinInforme { get; set; } = new();
    public List<string> PublicadoresSinInforme { get; set; } = new();
}

// Importar Excel
public class ImportarInformesDto
{
    public int Ano { get; set; }
    public int Mes { get; set; }
    public Guid IdGrupo { get; set; }
}

// Fila del Excel
public class InformeExcelRowDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public bool Participo { get; set; }
    public int? Horas { get; set; }
    public int Cursos { get; set; }
    public bool Inactivo { get; set; }
    public string? Observacion { get; set; }
}

// Resultado importación
public class ResultadoImportacionDto
{
    public int Exitosos { get; set; }
    public int Fallidos { get; set; }
    public List<string> Errores { get; set; } = new();
}