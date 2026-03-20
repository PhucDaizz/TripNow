using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Application.DTOs.UserSearchHistory;
using RecommendationService.Application.Features.UserSearchHistory.Commands.CreateUserSearchHistory;
using RecommendationService.Application.Features.UserSearchHistory.Queries.GetUserSearchHistory;

namespace RecommendationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserSearchHistoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public UserSearchHistoryController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserSearchHistoryCommand createUserSearchHistoryCommand)
        {
            var userId = _currentUserService.UserId;
            createUserSearchHistoryCommand.UserId = Guid.Parse(userId!);
            var result = await _mediator.Send(createUserSearchHistoryCommand);
            return Ok(ApiResponse<UserSearchHistoryDto>.SuccessResponse(result.Value));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserSearchHistory(Guid userId)
        {
            var query = new GetUserSearchHistoryQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<UserSearchHistoryDto>>.SuccessResponse(result.Value));
        }
    }
}
