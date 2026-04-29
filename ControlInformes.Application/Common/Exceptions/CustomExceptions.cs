namespace ControlInformes.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"La entidad \"{name}\" con clave ({key}) no fue encontrada.") { }
}

public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException(List<string> errors)
        : base("Se encontraron uno o más errores de validación.")
    {
        Errors = errors;
    }
}
