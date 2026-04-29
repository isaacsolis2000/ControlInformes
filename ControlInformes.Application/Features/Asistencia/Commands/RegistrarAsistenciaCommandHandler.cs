using ControlInformes.Domain.Interfaces;
using MediatR;

namespace ControlInformes.Application.Features.Asistencia.Commands;

public class RegistrarAsistenciaCommandHandler : IRequestHandler<RegistrarAsistenciaCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarAsistenciaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(RegistrarAsistenciaCommand request, CancellationToken cancellationToken)
    {
        var asistencia = new Domain.Entities.Asistencia
        {
            IdAsistencia = Guid.NewGuid(),
            Fecha = request.Fecha,
            TipoReunion = request.TipoReunion,
            Cantidad = request.Cantidad
        };

        await _unitOfWork.Asistencias.AddAsync(asistencia);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return asistencia.IdAsistencia;
    }
}
