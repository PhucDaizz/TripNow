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
    }
}
