using AutoMapper;
using ControlInformes.Application.DTOs;
using ControlInformes.Domain.Interfaces;
using MediatR;

namespace ControlInformes.Application.Features.Informes.Queries;

public class ObtenerInformesPorMesQueryHandler : IRequestHandler<ObtenerInformesPorMesQuery, IReadOnlyList<InformeMensualDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ObtenerInformesPorMesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<InformeMensualDto>> Handle(ObtenerInformesPorMesQuery request, CancellationToken cancellationToken)
    {
        var informes = await _unitOfWork.InformesMensuales.GetByMesAsync(request.Ano, request.Mes);
        return _mapper.Map<IReadOnlyList<InformeMensualDto>>(informes);
    }
}
