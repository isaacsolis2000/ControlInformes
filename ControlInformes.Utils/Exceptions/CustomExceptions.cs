namespace ControlInformes.Utils.Exceptions;

public class NotFoundException : Exception
{
    public string CodigoError { get; }

    public NotFoundException(string entidad, object clave)
        : base($"La entidad \"{entidad}\" con clave ({clave}) no fue encontrada.")
    {
        CodigoError = ErrorCatalog.EntidadNoEncontrada;
    }
}

public class BusinessValidationException : Exception
{
    public string CodigoError { get; }
    public List<string> Errores { get; }

    public BusinessValidationException(string mensaje, string? codigoError = null, List<string>? errores = null)
        : base(mensaje)
    {
        CodigoError = codigoError ?? ErrorCatalog.ValidacionFallida;
        Errores = errores ?? new List<string>();
    }

    public BusinessValidationException(List<string> errores)
        : base("Se encontraron errores de validación.")
    {
        CodigoError = ErrorCatalog.ValidacionFallida;
        Errores = errores;
    }
}
