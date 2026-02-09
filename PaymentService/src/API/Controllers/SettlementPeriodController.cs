using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using PaymentService.Application.Features.SettlementPeriod.Commands.RetrySettlement;
using PaymentService.Application.Features.SettlementPeriod.Queries.GetMySettlements;
using PaymentService.Application.Features.SettlementPeriod.Queries.GetSettlementById;
using PaymentService.Domain.Common;

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
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> RetrySettlement([FromBody] RetrySettlementCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(ApiResponse<string>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<string>.SuccessResponse("Reconciliation has been successfully reactivated."));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] GetMySettlementsQuery query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse(result.Value)) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _mediator.Send(new GetSettlementByIdQuery(id));
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse(result.Value)) : NotFound(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }
    }
}
