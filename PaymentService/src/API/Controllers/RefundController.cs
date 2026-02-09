using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using PaymentService.Application.Common.Interfaces;
using PaymentService.Application.Features.RefundRequest.Queries.GetRefundRequest;
using PaymentService.Domain.Common;

namespace PaymentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public RefundController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        [HttpGet]
        public async Task<IActionResult> GetRequests([FromQuery] GetRefundRequestsQuery query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse(result)) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }

        [Authorize] 
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests([FromQuery] GetRefundRequestsQuery query)
        {
            var userIdString = _currentUserService.UserId;

            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();

            query.UserId = Guid.Parse(userIdString);

            var result = await _mediator.Send(query);
            return result.IsSuccess
                ? Ok(ApiResponse<object>.SuccessResponse(result.Value))
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }

    }
}
