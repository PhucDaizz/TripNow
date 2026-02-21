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

        /// <summary>
        /// Admin thử lại kỳ đối soát cho chủ khách sạn khi trường hơp hệt thống bị lỗi
        /// </summary>
        /// <remarks>
        /// Chỉ admin dùng
        /// </remarks>
        [HttpPost("retry")]
        [Authorize(Roles = $"{AppRoles.SysAdmin}")]
        public async Task<IActionResult> RetrySettlement([FromBody] RetrySettlementCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure) return BadRequest(ApiResponse<string>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<string>.SuccessResponse("Reconciliation has been successfully reactivated."));
        }

        /// <summary>
        /// Xem danh sách kỳ đối soát
        /// </summary>
        /// <remarks>
        /// Chỉ admin, và chủ khách sạn dùng
        /// - Admin xem tất cả, chủ khách san chỉ xem của họ
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.SysAdmin}, {AppRoles.HotelOwner}")]
        public async Task<IActionResult> GetList([FromQuery] GetMySettlementsQuery query)
        {
            var result = await _mediator.Send(query);
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse(result.Value)) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }

        /// <summary>
        /// Xem chi tiết kỳ đối soát
        /// </summary>
        /// <remarks>
        /// Chỉ admin, và chủ khách sạn dùng
        /// - Admin xem tất cả, chủ khách sạn chỉ xem của họ
        /// </remarks>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _mediator.Send(new GetSettlementByIdQuery(id));
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse(result.Value)) : NotFound(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }
    }
}
