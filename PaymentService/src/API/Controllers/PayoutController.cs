using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using PaymentService.Application.Features.Payout.Commands.AcceptPayout;
using PaymentService.Application.Features.Payout.Commands.RejectPayout;
using PaymentService.Application.Features.Payout.Queries.GetOwnerPayouts;
using PaymentService.Application.Features.Payout.Queries.GetPayouts;
using PaymentService.Domain.Common;

namespace PaymentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayoutController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PayoutController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("admin")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> GetPayouts([FromQuery] GetPayoutsQuery query)
        {
            if (!query.Status.HasValue) query.Status = Domain.Enum.PayoutStatus.Pending; 

            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result);
        }

        [HttpPost("admin/reject")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> RejectPayout([FromBody]RejectPayoutCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<object>.SuccessResponse("Complete refund rejection"));
        }

        [HttpPost("admin/accept")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> AcceptPayout([FromBody]AcceptPayoutCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailure)
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));

            return Ok(ApiResponse<string>.SuccessResponse("Payment completed"));
        }

        [HttpGet("my-payouts")] 
        [Authorize(Roles = $"{AppRoles.HotelOwner}")] 
        public async Task<IActionResult> GetMyPayouts([FromQuery] GetOwnerPayoutsQuery query)
        {
            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result);
        }

    }
}
