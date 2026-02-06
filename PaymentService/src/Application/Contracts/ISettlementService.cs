namespace PaymentService.Application.Contracts
{
    public interface ISettlementService
    {
        Task RunSettlementJob(CancellationToken token = default);
        Task<bool> ProcessSettlementForOwnerAsync(Guid ownerId, CancellationToken token);
    }
}
