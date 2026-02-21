using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.OwnerBankAccount;
using PaymentService.Domain.Common;

namespace PaymentService.Application.Features.OwnerBankAccount.Queries.GetOwnerBankAccounts
{
    public class GetOwnerBankAccountsQueryHandler : IRequestHandler<GetOwnerBankAccountsQuery, Result<List<OwnerBankAccountDto?>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetOwnerBankAccountsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<OwnerBankAccountDto>>> Handle(GetOwnerBankAccountsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId);
            var role = _currentUserService.Role;

            bool isAdmin = role == AppRoles.SysAdmin;

            Guid targetOwnerId = currentUserId;

            if (isAdmin)
            {
                if (request.OwnerId.HasValue)
                {
                    targetOwnerId = request.OwnerId.Value;
                }
            }
            else
            {
                if (request.OwnerId.HasValue && request.OwnerId.Value != currentUserId)
                {
                    return Result.Failure<List<OwnerBankAccountDto>>(new Error("FORBIDDEN", "You are only allowed to view your own bank account."));
                }
            }

            var ownerBankAccounts = await _unitOfWork.OwnerBankAccounts.GetAllByOwnerId(targetOwnerId);

            var ownerBankAccountDtos = ownerBankAccounts.Select(oba => new OwnerBankAccountDto
            {
                OwnerId = oba.OwnerId,
                BankName = oba.BankName,
                BankAccountNumber = oba.BankAccountNumber,
                BankAccountHolder = oba.BankAccountHolder,
                IsDefault = oba.IsDefault
            }).OrderByDescending(x => x.IsDefault).ToList();

            return Result.Success(ownerBankAccountDtos);
        }
    }
}
