namespace ControlInformes.Domain.Enums;

public enum TipoPublicador
{
    Publicador = 0,
    NoBautizado = 1,
    PrecursorAuxiliar = 2,
    PrecursorRegular = 3
}
public enum TipoResumenPublicador
{
    Publicador = 0, // Incluye Publicador + NoBautizado
    PrecursorAuxiliar = 1,
    PrecursorRegular = 2
}