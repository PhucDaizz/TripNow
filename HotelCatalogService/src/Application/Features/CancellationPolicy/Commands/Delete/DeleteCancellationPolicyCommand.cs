using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.CancellationPolicy.Commands.Delete
{
    public class DeleteCancellationPolicyCommand : IRequest<Result<object>>
    {
        public Guid Id { get; set; }
    }

    public class DeleteCancellationPolicyCommandHandler : IRequestHandler<DeleteCancellationPolicyCommand, Result<object>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCancellationPolicyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<object>> Handle(DeleteCancellationPolicyCommand request, CancellationToken cancellationToken)
        {
            var entity = await _unitOfWork.CancellationPolicy.GetByIdAsync(request.Id, cancellationToken);

            if (entity == null)
            {
                return Result<object>.Failure(new Error("NOT.FOUND","CancellationPolicy not found."));
            }

            await _unitOfWork.CancellationPolicy.DeleteAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(Unit.Value);
        }
    }
}
