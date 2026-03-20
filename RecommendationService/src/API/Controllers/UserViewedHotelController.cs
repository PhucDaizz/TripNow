using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Application.DTOs.UserViewedHotel;
using RecommendationService.Application.Features.UserViewedHotel.Queries.GetUserViewedHotel;

namespace RecommendationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserViewedHotelController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public UserViewedHotelController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserViewedHotels(Guid userId)
        {
            var query = new GetUserViewedHotelQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<IEnumerable<UserViewedHotelDto>>.SuccessResponse(result.Value));
        }
    }
}
