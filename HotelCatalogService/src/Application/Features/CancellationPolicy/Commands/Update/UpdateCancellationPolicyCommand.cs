using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using HotelCatalogService.Domain.Enum;
using MediatR;

namespace HotelCatalogService.Application.Features.CancellationPolicy.Commands.Update
{
    public class UpdateCancellationPolicyCommand : IRequest<Result<object>>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CancellationPolicyType Type { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateCancellationPolicyCommandHandler : IRequestHandler<UpdateCancellationPolicyCommand, Result<object>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCancellationPolicyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<object>> Handle(UpdateCancellationPolicyCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.CancellationPolicy.GetByIdAsync(request.Id, cancellationToken);

            if (entity == null)
            {
                return Result<object>.Failure(new Error("NOT.FOUND", "CancellationPolicy not found."));
            }

            entity.Update(request.Name, request.Description, request.Type);
            await _unitOfWork.CancellationPolicy.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(Unit.Value);
        }
    }
}
