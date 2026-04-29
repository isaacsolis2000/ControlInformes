using AutoMapper;
using ControlInformes.Application.DTOs;
using ControlInformes.Domain.Interfaces;
using MediatR;

namespace ControlInformes.Application.Features.Informes.Queries;

public class ObtenerHistorialPublicadorQueryHandler : IRequestHandler<ObtenerHistorialPublicadorQuery, IReadOnlyList<InformeMensualDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ObtenerHistorialPublicadorQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<InformeMensualDto>> Handle(ObtenerHistorialPublicadorQuery request, CancellationToken cancellationToken)
    {
        var informes = await _unitOfWork.InformesMensuales.GetByPublicadorAsync(request.IdPublicador);
        return _mapper.Map<IReadOnlyList<InformeMensualDto>>(informes);
    }
}
