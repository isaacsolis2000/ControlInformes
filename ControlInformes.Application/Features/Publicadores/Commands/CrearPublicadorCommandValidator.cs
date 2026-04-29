using FluentValidation;

namespace ControlInformes.Application.Features.Publicadores.Commands;

public class CrearPublicadorCommandValidator : AbstractValidator<CrearPublicadorCommand>
{
    public CrearPublicadorCommandValidator()
    {
        RuleFor(x => x.NombreCompleto)
            .NotEmpty().WithMessage("El nombre completo es obligatorio.")
            .MaximumLength(200);

        RuleFor(x => x.FechaNacimiento)
            .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.")
            .LessThan(DateTime.Now).WithMessage("La fecha de nacimiento debe ser anterior a hoy.");

        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("El tipo de publicador no es válido.");
    }
}
