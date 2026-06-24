using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using RecommendationService.Application.DTOs.Rag;
using RecommendationService.Application.Features.RAG.Queries.GetHotelChatContext;

namespace RecommendationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RagController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RagController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Lấy ngữ cảnh (tài liệu/chính sách) của một khách sạn cụ thể dựa trên câu hỏi của người dùng
        /// </summary>
        /// <param name="hotelId">ID của khách sạn đang chat</param>
        /// <param name="request">Chứa câu hỏi và số lượng đoạn text muốn lấy</param>
        [HttpPost("hotel/{hotelId:guid}/context")]
        public async Task<IActionResult> GetHotelContext(Guid hotelId, [FromBody] GetHotelChatContextRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("The question cannot be left blank."));
            }

            var query = new GetHotelChatContextQuery(hotelId, request.Message, request.Limit);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));
            }

            return Ok(ApiResponse<List<string>>.SuccessResponse(result.Value));
        }
    }
}
