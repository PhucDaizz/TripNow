using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Features.RefundRequest.Queries.GetRefundRequest;
using PaymentService.Domain.Common;

namespace PaymentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RefundController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        [HttpGet]
        public async Task<IActionResult> GetRequests([FromQuery] GetRefundRequestsQuery query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
        }


    }
}
