namespace ControlInformes.Utils;

public static class ErrorCatalog
{
    // Entidad
    public const string EntidadNoEncontrada = "ENT_001";
    public const string EntidadDuplicada = "ENT_002";

    // Validación
    public const string ValidacionFallida = "VAL_001";
    public const string TipoPublicadorInvalido = "VAL_002";
    public const string HorasSoloParaPrecursores = "VAL_003";
    public const string PublicadorYaEnGrupo = "VAL_004";
    public const string CapitanNoValido = "VAL_005";

    // Archivo
    public const string ArchivoInvalido = "EXC_001";
    public const string NombreVacio = "EXC_002";

    // Sistema
    public const string ErrorInterno = "SYS_001";

    private static readonly Dictionary<string, string> _mensajes = new()
    {
        { EntidadNoEncontrada,      "La entidad solicitada no fue encontrada." },
        { EntidadDuplicada,         "Ya existe un registro con los mismos datos." },
        { ValidacionFallida,        "Se encontraron errores de validación." },
        { TipoPublicadorInvalido,   "El tipo de publicador no es válido." },
        { HorasSoloParaPrecursores, "Las horas solo aplican a precursores." },
        { PublicadorYaEnGrupo,      "El publicador ya pertenece a otro grupo." },
        { CapitanNoValido,          "El capitán asignado no es un publicador válido." },
        { ArchivoInvalido,          "El archivo proporcionado no es válido." },
        { NombreVacio,              "El nombre no puede estar vacío." },
        { ErrorInterno,             "Error interno del servidor." }
    };

    public static string GetMensaje(string codigo)
        => _mensajes.TryGetValue(codigo, out var mensaje) ? mensaje : "Error desconocido.";
}