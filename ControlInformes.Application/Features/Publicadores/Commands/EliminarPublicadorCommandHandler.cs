using ControlInformes.Application.Common.Exceptions;
using ControlInformes.Domain.Interfaces;
using MediatR;

namespace ControlInformes.Application.Features.Publicadores.Commands;

public class EliminarPublicadorCommandHandler : IRequestHandler<EliminarPublicadorCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public EliminarPublicadorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(EliminarPublicadorCommand request, CancellationToken cancellationToken)
    {
        var publicador = await _unitOfWork.Publicadores.GetByIdAsync(request.IdPublicador)
            ?? throw new NotFoundException(nameof(Domain.Entities.Publicador), request.IdPublicador);

        _unitOfWork.Publicadores.Remove(publicador);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
