using PaymentService.Domain.Common;

namespace PaymentService.Domain.Entities
{
    public class OwnerBankAccount: BaseEntity, AggregateRoot
    {
        public Guid OwnerId { get; private set; }
        public string BankName { get; private set; }      // VCB, TCB...
        public string BankAccountNumber { get; private set; }
        public string BankAccountHolder { get; private set; }
        public bool IsDefault { get; private set; }

        private OwnerBankAccount() { }

        public OwnerBankAccount(Guid ownerId, string bankName, string accNum, string holder, bool isDefault = false)
        {
            OwnerId = ownerId;
            BankName = bankName;
            BankAccountNumber = accNum;
            BankAccountHolder = holder;
            IsDefault = isDefault;
        }

        public void UpdateDetails(string bankName, string accNum, string holder)
        {
            BankName = bankName;
            BankAccountNumber = accNum;
            BankAccountHolder = holder;
        }

        public void SetAsDefault()
        {
            IsDefault = true;
        }

        public void RemoveDefault()
        {
            IsDefault = false;
        }
    }
}
