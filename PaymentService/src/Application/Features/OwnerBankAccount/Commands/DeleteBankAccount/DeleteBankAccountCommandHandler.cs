using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;

namespace PaymentService.Application.Features.OwnerBankAccount.Commands.DeleteBankAccount
{
    public class DeleteBankAccountCommandHandler : IRequestHandler<DeleteBankAccountCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteBankAccountCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteBankAccountCommand request, CancellationToken cancellationToken)
        {
            var ownerId = Guid.Parse(_currentUserService.UserId);
            var account = await _unitOfWork.OwnerBankAccounts.GetByIdAsync(request.Id);

            if (account == null || account.OwnerId != ownerId)
                return Result.Failure(new Error("Bank.NotFound", "Cannot find information for this bank."));

            if (account.IsDefault)
                return Result.Failure(new Error("Bank.CannotDeleteDefault", "Unable to delete the default account. Please select another account as the default first."));

            await _unitOfWork.OwnerBankAccounts.DeleteAsync(account);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
