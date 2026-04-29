using ControlInformes.Domain.Entities;
using ControlInformes.Domain.Interfaces;
using MediatR;

namespace ControlInformes.Application.Features.Publicadores.Commands;

public class CrearPublicadorCommandHandler : IRequestHandler<CrearPublicadorCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CrearPublicadorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CrearPublicadorCommand request, CancellationToken cancellationToken)
    {
        var publicador = new Publicador
        {
            IdPublicador = Guid.NewGuid(),
            NombreCompleto = request.NombreCompleto,
            FechaNacimiento = request.FechaNacimiento,
            FechaBautismo = request.FechaBautismo,
            Tipo = request.Tipo,
            Activo = true
        };

        await _unitOfWork.Publicadores.AddAsync(publicador);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return publicador.IdPublicador;
    }
}
