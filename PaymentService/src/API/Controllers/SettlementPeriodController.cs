using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using PaymentService.Application.Features.SettlementPeriod.Commands.RetrySettlement;

namespace PaymentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettlementPeriodController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SettlementPeriodController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("retry")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RetrySettlement([FromBody] RetrySettlementCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(ApiResponse<string>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<string>.SuccessResponse("Reconciliation has been successfully reactivated."));
        }
    }
}
