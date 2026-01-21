using HotelCatalogService.Application.Features.CancellationPolicy.Commands.Create;
using HotelCatalogService.Application.Features.CancellationPolicy.Commands.Delete;
using HotelCatalogService.Application.Features.CancellationPolicy.Commands.Rules.AddRule;
using HotelCatalogService.Application.Features.CancellationPolicy.Commands.Rules.RemoveRule;
using HotelCatalogService.Application.Features.CancellationPolicy.Commands.Rules.UpdateRule;
using HotelCatalogService.Application.Features.CancellationPolicy.Commands.Update;
using HotelCatalogService.Application.Features.CancellationPolicy.Queries.GetByHotel;
using HotelCatalogService.Application.Features.CancellationPolicy.Queries.GetById;
using HotelCatalogService.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace HotelCatalogService.API.Controllers
{
    [ApiController]
    public class CancellationPoliciesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CancellationPoliciesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách chính sách hủy của khách sạn.
        /// </summary>
        [HttpGet("api/Hotel/{hotelId}/cancellation-policies")]
        public async Task<IActionResult> GetByHotel(Guid hotelId)
        {
            var result = await _mediator.Send(new GetCancellationPoliciesByHotelQuery { HotelId = hotelId });
            return result.IsSuccess 
                ? Ok(ApiResponse<object>.SuccessResponse(result.Value)) 
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Lấy chi tiết một chính sách hủy theo ID.
        /// </summary>
        [HttpGet("api/cancellation-policies/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCancellationPolicyByIdQuery { Id = id });
            return result.IsSuccess 
                ? Ok(ApiResponse<object>.SuccessResponse(result.Value)) 
                : NotFound(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Tạo mới chính sách hủy cho khách sạn (Cần quyền HotelOwner).
        /// </summary>
        [HttpPost("api/Hotel/{hotelId}/cancellation-policies")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Create(Guid hotelId, [FromBody] CreateCancellationPolicyCommand command)
        {
            if (hotelId != command.HotelId) command.HotelId = hotelId;
            var result = await _mediator.Send(command);
            return result.IsSuccess 
                ? Created("", ApiResponse<object>.SuccessResponse(result.Value)) 
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Cập nhật thông tin chính sách hủy (Cần quyền HotelOwner).
        /// </summary>
        [HttpPut("api/cancellation-policies/{id}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCancellationPolicyCommand command)
        {
            if (id != command.Id) command.Id = id;
            var result = await _mediator.Send(command);
            return result.IsSuccess 
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Policy updated successfully.")) 
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Xóa chính sách hủy (Cần quyền HotelOwner).
        /// </summary>
        [HttpDelete("api/cancellation-policies/{id}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCancellationPolicyCommand { Id = id });
            return result.IsSuccess 
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Policy deleted successfully.")) 
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Thêm một quy tắc hủy mới vào chính sách (Cần quyền HotelOwner).
        /// </summary>
        [HttpPost("api/cancellation-policies/{id}/rules")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> AddRule(Guid id, [FromBody] AddCancellationRuleCommand command)
        {
            if (id != command.CancellationPolicyId) command.CancellationPolicyId = id;
            var result = await _mediator.Send(command);
            return result.IsSuccess 
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Rule added successfully.")) 
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Cập nhật quy tắc hủy (Cần quyền HotelOwner).
        /// </summary>
        [HttpPut("api/cancellation-policies/{id}/rules/{ruleId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> UpdateRule(Guid id, Guid ruleId, [FromBody] UpdateCancellationRuleCommand command)
        {
            if (id != command.CancellationPolicyId) command.CancellationPolicyId = id;
            if (ruleId != command.RuleId) command.RuleId = ruleId;
            
            var result = await _mediator.Send(command);
            return result.IsSuccess 
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Rule updated successfully.")) 
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }

        /// <summary>
        /// Xóa quy tắc hủy khỏi chính sách (Cần quyền HotelOwner).
        /// </summary>
        [HttpDelete("api/cancellation-policies/{id}/rules/{ruleId}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> RemoveRule(Guid id, Guid ruleId)
        {
            var result = await _mediator.Send(new RemoveCancellationRuleCommand { CancellationPolicyId = id, RuleId = ruleId });
            return result.IsSuccess 
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Rule removed successfully.")) 
                : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
        }
    }
}
