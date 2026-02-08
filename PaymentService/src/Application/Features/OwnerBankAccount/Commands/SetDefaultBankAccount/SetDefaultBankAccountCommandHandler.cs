using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;

namespace PaymentService.Application.Features.OwnerBankAccount.Commands.SetDefaultBankAccount
{
    public class SetDefaultBankAccountCommandHandler : IRequestHandler<SetDefaultBankAccountCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public SetDefaultBankAccountCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(SetDefaultBankAccountCommand request, CancellationToken token)
        {
            var ownerId = Guid.Parse(_currentUserService.UserId);

            var targetAccount = await _unitOfWork.OwnerBankAccounts.GetByIdAsync(request.Id);
            if (targetAccount == null || targetAccount.OwnerId != ownerId)
                return Result.Failure(new Error("Bank.NotFound", "Cannot find information for this bank."));

            if (targetAccount.IsDefault) return Result.Success(); 

            var oldDefault = await _unitOfWork.OwnerBankAccounts.GetDefaultByOwnerIdAsync(ownerId);
            if (oldDefault != null)
            {
                oldDefault.RemoveDefault();
                await _unitOfWork.OwnerBankAccounts.UpdateAsync(oldDefault);
            }

            targetAccount.SetAsDefault();
            await _unitOfWork.OwnerBankAccounts.UpdateAsync(targetAccount);

            await _unitOfWork.SaveChangesAsync(token);
            return Result.Success();
        }
    }
}
