using Domain.Common.Response;
using MediatR;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.DTOs.OwnerBankAccount;

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
            var ownerId = _currentUserService.UserId;

            var ownerBankAccounts = await _unitOfWork.OwnerBankAccounts.GetAllByOwnerId(Guid.Parse(ownerId));

            var ownerBankAccountDtos = ownerBankAccounts.Select(oba => new OwnerBankAccountDto
            {
                OwnerId = oba.OwnerId,
                BankName = oba.BankName,
                BankAccountNumber = oba.BankAccountNumber,
                BankAccountHolder = oba.BankAccountHolder,
                IsDefault = oba.IsDefault
            }).OrderByDescending(x => x.IsDefault).ToList();

            return Result<List<OwnerBankAccountDto>>.Success(ownerBankAccountDtos);
        }
    }
}
