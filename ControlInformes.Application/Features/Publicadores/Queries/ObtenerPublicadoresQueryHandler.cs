using AutoMapper;
using ControlInformes.Application.DTOs;
using ControlInformes.Domain.Interfaces;
using MediatR;

namespace ControlInformes.Application.Features.Publicadores.Queries;

public class ObtenerPublicadoresQueryHandler : IRequestHandler<ObtenerPublicadoresQuery, IReadOnlyList<PublicadorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ObtenerPublicadoresQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PublicadorDto>> Handle(ObtenerPublicadoresQuery request, CancellationToken cancellationToken)
    {
        var publicadores = await _unitOfWork.Publicadores.GetAllAsync();
        return _mapper.Map<IReadOnlyList<PublicadorDto>>(publicadores);
    }
}
