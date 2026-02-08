using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;

namespace PaymentService.Application.Features.OwnerBankAccount.Commands.UpdateBankAccount
{
    public class UpdateBankAccountCommandHandler : IRequestHandler<UpdateBankAccountCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateBankAccountCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateBankAccountCommand request, CancellationToken token)
        {
            var ownerId = Guid.Parse(_currentUserService.UserId);

            var account = await _unitOfWork.OwnerBankAccounts.GetByIdAsync(request.Id);

            if (account == null) return Result.Failure(new Error("Bank.NotFound", "Cannot find information for this bank."));

            if (account.OwnerId != ownerId) return Result.Failure(new Error("Auth.Forbidden", "you are not allowed to modify this information"));

            account.UpdateDetails(request.BankName, request.BankAccountNumber, request.BankAccountHolder);

            await _unitOfWork.OwnerBankAccounts.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success();
        }
    }
}
