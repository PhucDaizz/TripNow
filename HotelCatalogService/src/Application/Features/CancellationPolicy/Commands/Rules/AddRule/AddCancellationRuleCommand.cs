using Domain.Common.Response;
using HotelCatalogService.Application.Common.Interfaces;
using MediatR;

namespace HotelCatalogService.Application.Features.CancellationPolicy.Commands.Rules.AddRule
{
    public class AddCancellationRuleCommand : IRequest<Result<object>>
    {
        public Guid CancellationPolicyId { get; set; }
        public int HoursBeforeCheckIn { get; set; }
        public decimal RefundPercentage { get; set; }
    }

    public class AddCancellationRuleCommandHandler : IRequestHandler<AddCancellationRuleCommand, Result<object>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddCancellationRuleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<object>> Handle(AddCancellationRuleCommand request, CancellationToken cancellationToken)
        {
            var policy = await _unitOfWork.CancellationPolicy.GetByIdAsync(request.CancellationPolicyId, cancellationToken);

            if (policy == null)
            {
                return Result<object>.Failure(new Error("NOT.FOUND", "CancellationPolicy not found."));
            }

            try
            {
                policy.AddRule(request.HoursBeforeCheckIn, request.RefundPercentage);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<object>.Success(Unit.Value);
            }
            catch (ArgumentException ex)
            {
                return Result<object>.Failure(new Error("ERROR", ex.Message));
            }
        }
    }
}
