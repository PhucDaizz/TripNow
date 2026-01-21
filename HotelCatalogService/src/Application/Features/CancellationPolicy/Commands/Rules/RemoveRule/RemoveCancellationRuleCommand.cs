using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.CancellationPolicy.Commands.Rules.RemoveRule
{
    public class RemoveCancellationRuleCommand : IRequest<Result<object>>
    {
        public Guid CancellationPolicyId { get; set; }
        public Guid RuleId { get; set; }
    }

    public class RemoveCancellationRuleCommandHandler : IRequestHandler<RemoveCancellationRuleCommand, Result<object>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveCancellationRuleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<object>> Handle(RemoveCancellationRuleCommand request, CancellationToken cancellationToken)
        {
            var policy = await _unitOfWork.CancellationPolicy.GetByIdAsync(request.CancellationPolicyId, cancellationToken);

            if (policy == null)
            {
                return Result<object>.Failure(new Error("NOT.FOUND", "CancellationPolicy not found."));
            }

            policy.RemoveRule(request.RuleId);
            await _unitOfWork.CancellationPolicy.UpdateAsync(policy, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(Unit.Value);
        }
    }
}
