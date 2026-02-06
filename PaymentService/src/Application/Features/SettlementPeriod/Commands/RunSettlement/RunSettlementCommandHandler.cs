using Domain.Common.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Contracts;

namespace PaymentService.Application.Features.SettlementPeriod.Commands.RunSettlement
{
    public class RunSettlementCommandHandler : IRequestHandler<RunSettlementCommand, Result>
    {
        private readonly ISettlementService _settlementService;
        private readonly ILogger<RunSettlementCommandHandler> _logger;

        public RunSettlementCommandHandler(ISettlementService settlementService, ILogger<RunSettlementCommandHandler> logger)
        {
            _settlementService = settlementService;
            _logger = logger;
        }

        public async Task<Result> Handle(RunSettlementCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handler bắt đầu chạy đối soát...");

            await _settlementService.RunSettlementJob();

            return Result.Success();
        }
    }
}
