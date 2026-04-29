namespace ControlInformes.Utils;

public class ApiResponse<T>
{
    public int HttpCode { get; set; }
    public T? Result { get; set; }
    public bool HasError { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public string? CodigoError { get; set; }
    public List<string>? Errores { get; set; }

    public static ApiResponse<T> Ok(T data, string mensaje = "Operación exitosa", int httpCode = 200)
        => new() { HttpCode = httpCode, Result = data, HasError = false, Mensaje = mensaje };

    public static ApiResponse<T> Fail(string mensaje, string? codigoError = null, int httpCode = 400, List<string>? errores = null)
        => new() { HttpCode = httpCode, HasError = true, Mensaje = mensaje, CodigoError = codigoError, Errores = errores };

    public static ApiResponse<T> NotFound(string mensaje, string? codigoError = "NOT_FOUND")
        => new() { HttpCode = 404, HasError = true, Mensaje = mensaje, CodigoError = codigoError };

    public static ApiResponse<T> Error(string mensaje, string? codigoError = "INTERNAL_ERROR")
        => new() { HttpCode = 500, HasError = true, Mensaje = mensaje, CodigoError = codigoError };
}
