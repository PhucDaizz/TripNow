using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using PaymentService.Application.Features.OwnerWallet.Queries.GetMyWallet;
using PaymentService.Application.Features.OwnerWallet.Queries.GetWalletTransactions;

namespace PaymentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WalletController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _mediator.Send(new GetMyWalletQuery());
            return Ok(ApiResponse<object>.SuccessResponse(result.Value));
        }

        [Authorize]
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] GetWalletTransactionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<object>.SuccessResponse(result.Value));
        }
    }
}
