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

        /// <summary>
        /// Bắt đầu một cuộc trò chuyện mới hoặc lấy Id cuộc trò chuyện hiện có giữa khách hàng và khách sạn. Nếu cuộc trò chuyện đã tồn tại, nó sẽ được trả về thay vì tạo một cuộc trò chuyện mới.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
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

        /// <summary>
        /// Lấy tất cả cuộc trò chuyện liên quan đến người dùng hiện tại, bao gồm cả vai trò của họ (khách hàng, nhân viên khách sạn hoặc quản trị hệ thống). Kết quả được phân trang để quản lý hiệu quả số lượng lớn cuộc trò chuyện. Người dùng có thể xem các cuộc trò chuyện mà họ tham gia, với thông tin về vai trò của họ trong mỗi cuộc trò chuyện. Điều này cho phép người dùng dễ dàng theo dõi và quản lý các cuộc trò chuyện của mình dựa trên vai trò và mối quan hệ với khách sạn.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
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


        /// <summary>
        /// Lấy lịch sử trò chuyện cho một cuộc trò chuyện cụ thể, được xác định bởi Id cuộc trò chuyện. Kết quả được phân trang để quản lý hiệu quả số lượng lớn tin nhắn. Người dùng có thể xem các tin nhắn trước đó trong cuộc trò chuyện, với tùy chọn lọc theo thời gian (ví dụ: chỉ lấy tin nhắn trước một thời điểm nhất định) và giới hạn số lượng tin nhắn trả về trong mỗi lần truy vấn. Điều này giúp người dùng dễ dàng theo dõi và quản lý lịch sử trò chuyện của mình, đặc biệt là trong các cuộc trò chuyện dài hoặc có nhiều tin nhắn.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
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
