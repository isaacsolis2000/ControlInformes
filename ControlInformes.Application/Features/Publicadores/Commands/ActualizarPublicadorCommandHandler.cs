using ControlInformes.Application.Common.Exceptions;
using ControlInformes.Domain.Interfaces;
using MediatR;

namespace ControlInformes.Application.Features.Publicadores.Commands;

public class ActualizarPublicadorCommandHandler : IRequestHandler<ActualizarPublicadorCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarPublicadorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActualizarPublicadorCommand request, CancellationToken cancellationToken)
    {
        var publicador = await _unitOfWork.Publicadores.GetByIdAsync(request.IdPublicador)
            ?? throw new NotFoundException(nameof(Domain.Entities.Publicador), request.IdPublicador);

        publicador.NombreCompleto = request.NombreCompleto;
        publicador.FechaNacimiento = request.FechaNacimiento;
        publicador.FechaBautismo = request.FechaBautismo;
        publicador.Tipo = request.Tipo;
        publicador.Activo = request.Activo;

        _unitOfWork.Publicadores.Update(publicador);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
