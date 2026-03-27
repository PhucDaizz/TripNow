using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Application.Features.Recommendation.Queries;

namespace RecommendationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public RecommendationController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("for-user")]
        public async Task<IActionResult> GetRecommendations([FromQuery] int limit = 10)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var query = new GetPersonalizedRecommendationsQuery(Guid.Parse(userId), limit);
            var result = await _mediator.Send(query);

            return Ok(ApiResponse<IEnumerable<Guid>>.SuccessResponse(result.Value));
        }

        [HttpGet("hotel/{hotelId:guid}/similar")]
        public async Task<IActionResult> GetSimilarHotels(Guid hotelId, [FromQuery] int limit = 5)
        {
            var query = new GetSimilarHotelsQuery(hotelId, limit);
            var result = await _mediator.Send(query);

            if (result.IsFailure)
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.Message));

            return Ok(ApiResponse<IEnumerable<Guid>>.SuccessResponse(result.Value));
        }
    }
}
