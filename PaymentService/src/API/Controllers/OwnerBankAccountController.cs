using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using PaymentService.Application.Features.OwnerBankAccount.Commands.CreateBankAccount;
using PaymentService.Application.Features.OwnerBankAccount.Commands.DeleteBankAccount;
using PaymentService.Application.Features.OwnerBankAccount.Commands.SetDefaultBankAccount;
using PaymentService.Application.Features.OwnerBankAccount.Commands.UpdateBankAccount;
using PaymentService.Application.Features.OwnerBankAccount.Queries.GetOwnerBankAccounts;
using PaymentService.Domain.Common;

namespace PaymentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class OwnerBankAccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OwnerBankAccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy danh sách ngân hàng rút tiền về
        /// </summary>
        /// <remarks>
        /// Chủ khách sạn, Admin dùng. 
        /// Admin có thể truyền ownerId để xem của người khác.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.HotelOwner}, {AppRoles.SysAdmin}")]
        public async Task<IActionResult> GetOwnerBankAccount([FromQuery] Guid? ownerId = null)
        {
            var query = new GetOwnerBankAccountsQuery { OwnerId = ownerId };
            var result = await _mediator.Send(query);
            if (result.IsFailure)
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        /// <summary>
        /// Thêm tài khoản rút về
        /// </summary>
        /// <remarks>
        /// Chủ khách sạn dùng
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Create([FromBody] CreateBankAccountCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse(result.Value)) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }

        /// <summary>
        /// Cập nhật tài khoản rút về
        /// </summary>
        /// <remarks>
        /// Chủ khách sạn dùng
        /// </remarks>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBankAccountCommand command)
        {
            if (id != command.Id) return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch"));
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse("Update successfully")) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }

        /// <summary>
        /// Chọn tài khoản mật định sẽ rút về
        /// </summary>
        /// <remarks>
        /// Chủ khách sạn dùng
        /// </remarks>
        [HttpPatch("{id}/set-default")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> SetDefault(Guid id)
        {
            var result = await _mediator.Send(new SetDefaultBankAccountCommand { Id = id });
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse("Set as default successfully")) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }

        /// <summary>
        /// Xoá tài khoản rút về
        /// </summary>
        /// <remarks>
        /// Chủ khách sạn dùng
        /// </remarks>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.HotelOwner}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteBankAccountCommand { Id = id });
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse("Remove successfully")) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }
    }
}
