using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Contracts;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enum;
using PaymentService.Infrastructure.Settings;
using System.Text.Json;

namespace PaymentService.Infrastructure.Services
{
    public class SettlementService : ISettlementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SettlementService> _logger;
        private readonly PayoutSettings _config;

        public SettlementService(
            IUnitOfWork unitOfWork,
            ILogger<SettlementService> logger,
            IOptions<PayoutSettings> config)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _config = config.Value;
        }

        public async Task RunSettlementJob(CancellationToken token = default)
        {
            _logger.LogInformation("Settlement Job started");

            var ownerIds = await _unitOfWork.OwnerWallets.GetAllActiveOwnerIdsAsync(token);
            var countOwner = ownerIds.Count();
            int success = 0;

            foreach (var ownerId in ownerIds)
            {
                try
                {
                    var result = await ProcessSettlementForOwnerAsync(ownerId, token);
                    if (result) success++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Serious unrecoverable error for Owner: {OwnerId}", ownerId);
                }
            }

            _logger.LogInformation("Settlement Job completed. Success: {Count}/{Total}", success, countOwner);
        }

        public async Task<bool> ProcessSettlementForOwnerAsync(Guid ownerId, CancellationToken token)
        {
            var cutOffDate = DateTime.UtcNow.AddDays(-_config.HoldDays);

            int maxRetries = 3;
            for (int retryCount = 0; retryCount < maxRetries; retryCount++)
            {
                try
                {
                    var wallet = await _unitOfWork.OwnerWallets.GetWalletWithPendingLedgersAsync(ownerId, cutOffDate, token);

                    if (wallet == null) return false;

                    var unsetledLedgers = wallet.WalletLedgers.ToList();

                    if (!unsetledLedgers.Any())
                    {
                        return false; 
                    }

                    var period = new SettlementPeriod(
                        ownerId,
                        unsetledLedgers.Min(x => x.CreatedAt),
                        unsetledLedgers.Max(x => x.CreatedAt)
                    );

                    await _unitOfWork.SettlementPeriods.AddAsync(period);

                    foreach (var ledger in unsetledLedgers)
                    {
                        period.AddSettlementItem(
                            ledger.ReferenceId,
                            ledger.TransactionGrossAmount,
                            ledger.TransactionFee,
                            SettlementItemType.Booking
                        );
                        ledger.MarkAsSettled(period.Id);
                    }

                    wallet.ReleaseSettlement(period.Id, period.TotalNetPayable, period.TotalGross, period.TotalCommission);

                    period.MarkAsOpen();

                    var bankAccount = await _unitOfWork.OwnerBankAccounts.GetDefaultByOwnerIdAsync(ownerId);

                    if (bankAccount != null)
                    {
                        decimal actualPayoutAmount = wallet.AvailableBalance;

                        if (actualPayoutAmount > 0)
                        {
                            var bankInfoJson = JsonSerializer.Serialize(new
                            {
                                bankAccount.BankName,
                                bankAccount.BankAccountNumber,
                                bankAccount.BankAccountHolder
                            });

                            var payout = new Payout(period.Id, wallet.Id, actualPayoutAmount, bankInfoJson);
                            await _unitOfWork.Payouts.AddAsync(payout);

                            wallet.DebitForPayout(payout.Id, actualPayoutAmount, actualPayoutAmount, 0);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync(token);

                    _logger.LogInformation("Successfully processed Settlement for Owner: {Id}. Payout Period: {PeriodId}", ownerId, period.Id);

                    return true; 
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (retryCount == maxRetries - 1)
                    {
                        _logger.LogError("Continuous data collision error when Settlement for Owner: {OwnerId}. Cancelled.", ownerId);
                        return false;
                    }

                    foreach (var entry in ex.Entries)
                    {
                        await entry.ReloadAsync();
                    }

                    await Task.Delay(Random.Shared.Next(100, 300), token);
                }
            }

            return false;
        }

        /*public async Task RunSettlementJob(CancellationToken token = default)
        {
            _logger.LogInformation("Bắt đầu Job đối soát (Settlement Job)...");

            // 1. Xác định thời điểm chốt sổ (Cut-off Time)
            var cutOffDate = DateTime.UtcNow.AddDays(-_config.HoldDays);
            _logger.LogInformation("Ngày chốt sổ (Cut-off): {Date}", cutOffDate);

            var ownerIds = await _unitOfWork.OwnerWallets.GetAllActiveOwnerIdsAsync(token);

            int processedCount = 0;

            foreach (var ownerId in ownerIds)
            {
                var unsetledLedgers = await _unitOfWork.OwnerWallets
                    .GetPendingLedgersForSettlementAsync(ownerId, cutOffDate, token);

                if (!unsetledLedgers.Any()) continue;

                try
                {
                    var period = new SettlementPeriod(
                        ownerId,
                        unsetledLedgers.Min(x => x.CreatedAt),
                        unsetledLedgers.Max(x => x.CreatedAt)
                    );

                    await _unitOfWork.SettlementPeriods.AddAsync(period);

                    foreach (var ledger in unsetledLedgers)
                    {
                        period.AddSettlementItem(
                            ledger.ReferenceId,
                            ledger.TransactionGrossAmount, 
                            ledger.TransactionFee,         
                            SettlementItemType.Booking
                        );

                        ledger.MarkAsSettled(period.Id);

                    }

                    var wallet = await _unitOfWork.OwnerWallets.GetByOwnerIdAsync(ownerId, token);
                    if (wallet == null)
                    {
                        _logger.LogError("Wallet not found for OwnerId: {OwnerId}", ownerId);
                        continue;
                    }

                    var bankAccount = await _unitOfWork.OwnerBankAccounts.GetDefaultByOwnerIdAsync(ownerId);

                    wallet.ReleaseSettlement(
                        period.Id,
                        period.TotalNetPayable,  
                        period.TotalGross,       
                        period.TotalCommission   
                    );

                    if (bankAccount != null)
                    {
                        var bankInfoJson = JsonSerializer.Serialize(new
                        {
                            bankAccount.BankName,
                            bankAccount.BankAccountNumber,
                            bankAccount.BankAccountHolder
                        });

                        var payout = new Payout(
                            period.Id,          
                            wallet.Id,          
                            period.TotalNetPayable,
                            bankInfoJson
                        );

                        await _unitOfWork.Payouts.AddAsync(payout);

                        wallet.DebitForPayout(
                            payout.Id,               
                            period.TotalNetPayable,  
                            period.TotalNetPayable,  
                            0                        
                        );

                    }
                    else
                    {
                        _logger.LogWarning("Owner {Id} has not set up a bank account. Funds are held in the Available Wallet.", ownerId);
                    }

                    period.MarkAsOpen();

                    await _unitOfWork.SaveChangesAsync(token);

                    processedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during reconciliation for OwnerId: {OwnerId}", ownerId);
                }
            }

            _logger.LogInformation("Reconciliation Job completed. Processed {Count} Owners.", processedCount);
        }*/
    }
}
