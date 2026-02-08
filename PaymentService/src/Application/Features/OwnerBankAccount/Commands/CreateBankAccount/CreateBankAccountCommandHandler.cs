using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;

namespace PaymentService.Application.Features.OwnerBankAccount.Commands.CreateBankAccount
{
    public class CreateBankAccountCommandHandler : IRequestHandler<CreateBankAccountCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateBankAccountCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(CreateBankAccountCommand request, CancellationToken token)
        {
            var ownerId = Guid.Parse(_currentUserService.UserId);

            var existingCount = await _unitOfWork.OwnerBankAccounts.CountAsync(ownerId);
            if (existingCount == 0) request.IsDefault = true;

            if (request.IsDefault)
            {
                var defaultAcc = await _unitOfWork.OwnerBankAccounts.GetDefaultByOwnerIdAsync(ownerId, token);
                if (defaultAcc != null)
                {
                    defaultAcc.RemoveDefault();
                    await _unitOfWork.OwnerBankAccounts.UpdateAsync(defaultAcc);
                }
            }

            var newAccount = new Domain.Entities.OwnerBankAccount(
                ownerId,
                request.BankName,
                request.BankAccountNumber,
                request.BankAccountHolder,
                request.IsDefault
            );

            await _unitOfWork.OwnerBankAccounts.AddAsync(newAccount);
            await _unitOfWork.SaveChangesAsync(token);

            return Result.Success(newAccount.Id);
        }
    }
}
