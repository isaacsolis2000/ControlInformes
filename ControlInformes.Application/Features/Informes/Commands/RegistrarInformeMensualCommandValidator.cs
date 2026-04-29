using FluentValidation;

namespace ControlInformes.Application.Features.Informes.Commands;

public class RegistrarInformeMensualCommandValidator : AbstractValidator<RegistrarInformeMensualCommand>
{
    public RegistrarInformeMensualCommandValidator()
    {
        RuleFor(x => x.IdPublicador).NotEmpty();
        RuleFor(x => x.Ano).InclusiveBetween(2000, 2100);
        RuleFor(x => x.Mes).InclusiveBetween(1, 12);
        RuleFor(x => x.CursosBiblicos).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Horas).GreaterThanOrEqualTo(0).When(x => x.Horas.HasValue);
    }
}
