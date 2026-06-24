using ChatService.Application.DTOs.ChatBot;
using ChatService.Application.Features.ChatBot.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;

namespace ChatService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatBotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("hotel/{hotelId:guid}/ask")]
        public async Task<IActionResult> AskHotelBot(Guid hotelId, [FromBody] AskBotRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Câu hỏi không được để trống."));
            }

            var query = new AskHotelBotQuery(hotelId, request.Message);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
            }

            return Ok(ApiResponse<string>.SuccessResponse(result.Value));
        }
    }
}
