using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using PaymentService.Application.Features.OwnerBankAccount.Commands.CreateBankAccount;
using PaymentService.Application.Features.OwnerBankAccount.Commands.SetDefaultBankAccount;
using PaymentService.Application.Features.OwnerBankAccount.Commands.UpdateBankAccount;
using PaymentService.Application.Features.OwnerBankAccount.Queries.GetOwnerBankAccounts;

namespace PaymentService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OwnerBankAccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OwnerBankAccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetOwnerBankAccount()
        {
            var query = new GetOwnerBankAccountsQuery();
            var result = await _mediator.Send(query);
            if (result.IsFailure)
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBankAccountCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse(result)) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBankAccountCommand command)
        {
            if (id != command.Id) return BadRequest(ApiResponse<object>.ErrorResponse("ID mismatch"));
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(ApiResponse<object>.SuccessResponse(result)) : BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
        }

        [HttpPatch("{id}/set-default")]
        public async Task<IActionResult> SetDefault(Guid id)
        {
            var result = await _mediator.Send(new SetDefaultBankAccountCommand { Id = id });
            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteBankAccountCommand { Id = id });
            return result.IsSuccess ? Ok(result) : BadRequest(result.Error);
        }
    }
}
