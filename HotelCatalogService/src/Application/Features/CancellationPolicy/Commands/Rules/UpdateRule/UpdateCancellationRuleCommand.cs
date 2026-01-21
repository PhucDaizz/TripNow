using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.CancellationPolicy.Commands.Rules.UpdateRule
{
    public class UpdateCancellationRuleCommand : IRequest<Result<object>>
    {
        public Guid CancellationPolicyId { get; set; }
        public Guid RuleId { get; set; }
        public int HoursBeforeCheckIn { get; set; }
        public decimal RefundPercentage { get; set; }
    }

    public class UpdateCancellationRuleCommandHandler : IRequestHandler<UpdateCancellationRuleCommand, Result<object>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCancellationRuleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<object>> Handle(UpdateCancellationRuleCommand request, CancellationToken cancellationToken)
        {
            var policy = await _unitOfWork.CancellationPolicy.GetByIdAsync(request.CancellationPolicyId, cancellationToken);
                
            if (policy == null)
            {
                return Result<object>.Failure(new Error("NOT.FOUND", "CancellationPolicy not found."));
            }

            try 
            {
                policy.UpdateRule(request.RuleId, request.HoursBeforeCheckIn, request.RefundPercentage);
                await _unitOfWork.CancellationPolicy.UpdateAsync(policy, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<object>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                return Result<object>.Failure(new Error("NOT.FOUND", ex.Message));
            }
        }
    }
}
