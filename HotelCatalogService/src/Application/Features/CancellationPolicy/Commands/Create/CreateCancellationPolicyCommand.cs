using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Enum;
using MediatR;

namespace HotelCatalogService.Application.Features.CancellationPolicy.Commands.Create
{
    public class CreateCancellationPolicyCommand : IRequest<Result<Guid>>
    {
        public Guid HotelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public CancellationPolicyType Type { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class CreateCancellationPolicyCommandHandler : IRequestHandler<CreateCancellationPolicyCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCancellationPolicyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateCancellationPolicyCommand request, CancellationToken cancellationToken)
        {
            var entity = new Domain.Entities.CancellationPolicy(request.HotelId, request.Name, request.Description, request.Type);

            await _unitOfWork.CancellationPolicy.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(entity.Id);
        }
    }
}
