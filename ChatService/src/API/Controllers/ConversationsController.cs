using ChatService.Application.Common.Interfaces;
using ChatService.Application.DTOs.Conversation;
using ChatService.Application.DTOs.Message;
using ChatService.Application.Features.Conversation.Commands.CreateOrGetConversation;
using ChatService.Application.Features.Conversation.Queries.GetAll;
using ChatService.Application.Features.Conversation.Queries.GetChatHistory;
using ChatService.Domain.Common;
using ChatService.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nexus.BuildingBlocks.Model;
using System.Security.Claims;

namespace ChatService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ConversationsController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartConversation([FromBody]CreateOrGetConversationDto createOrGetConversationDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var currentUserId))
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("The token is invalid."));
            }

            var command = new CreateOrGetConversationCommand
            {
                HotelId = createOrGetConversationDto.HotelId,
                CurrentUserId = currentUserId
            };

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
            }

            return Ok(ApiResponse<Guid>.SuccessResponse(result.Value));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllConversation(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid currentUserId))
            {
                return Unauthorized("Invalid Token");
            }

            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var roleEnum = userRole switch
            {
                AppRoles.Customer => SenderType.Customer,
                AppRoles.Receptionist or AppRoles.HotelOwner or AppRoles.Housekeeping => SenderType.Hotel,
                AppRoles.SysAdmin => SenderType.SystemBot,
                _ => SenderType.Customer
            };



            var query = new GetAllConversationsQuery
            {
                CurrentUserId = currentUserId,
                CurrentUserRole = roleEnum,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var hotelId =  _currentUserService.HotelId;
            if (hotelId != null)
                query.HotelId = hotelId;


            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
            }

            return Ok(ApiResponse<Domain.Common.Models.PagedResult<ConversationListDto?>>.SuccessResponse(result.Value));
        }


        [HttpGet("{conversationId}/messages")]
        public async Task<IActionResult> GetChatHistory(
            Guid conversationId,
            [FromQuery] DateTime? before = null,
            [FromQuery] int size = 50)
        {
            var query = new GetChatHistoryQuery
            {
                ConversationId = conversationId,
                Before = before,
                Size = size
            };

            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(result.Error.ToString()));
            }

            return Ok(ApiResponse<List<MessageDto>>.SuccessResponse(result.Value));
        }
    }
}
