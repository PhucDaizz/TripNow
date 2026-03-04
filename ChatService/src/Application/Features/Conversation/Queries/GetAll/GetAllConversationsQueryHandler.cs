using ChatService.Application.Common.Interfaces;
using ChatService.Application.DTOs.Conversation;
using ChatService.Domain.Common.Models;
using ChatService.Domain.Entities;
using ChatService.Domain.Enum;
using Domain.Common.Response;
using MediatR;

namespace ChatService.Application.Features.Conversation.Queries.GetAll
{
    public class GetAllConversationsQueryHandler : IRequestHandler<GetAllConversationsQuery, Result<PagedResult<ConversationListDto?>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllConversationsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<ConversationListDto?>>> Handle(GetAllConversationsQuery request, CancellationToken cancellationToken)
        {
            List<Conversations> conversations = new();
            int totalCount = 0;

            if (request.CurrentUserRole == SenderType.Hotel)
            {
                if (!request.HotelId.HasValue)
                    return Result.Failure<PagedResult<ConversationListDto?>>(new Error("UNAUTHORIZED", "Lễ tân chưa được gán Khách sạn."));

                var dbResult = await _unitOfWork.Conversation.GetConversationsByHotelIdAsync(
                    request.HotelId.Value, request.PageIndex, request.PageSize, cancellationToken);

                conversations = dbResult.Items;
                totalCount = dbResult.TotalCount; 
            }
            else
            {
                var dbResult = await _unitOfWork.Conversation.GetConversationsByUserIdAsync(
                    request.CurrentUserId, request.PageIndex, request.PageSize, cancellationToken);

                conversations = dbResult.Items;
                totalCount = dbResult.TotalCount; 
            }

            var dtos = conversations.Select(c => new ConversationListDto
            {
                Id = c.Id,
                HotelId = c.HotelId,
                UserId = c.UserId,
                LastMessage = c.LastMessage ?? string.Empty,
                UpdatedAt = c.UpdatedAt ?? c.CreatedAt,
                UnreadCount = request.CurrentUserRole == SenderType.Customer
                              ? c.CustomerUnreadCount
                              : c.HotelUnreadCount,
                DisplayName = request.CurrentUserRole == SenderType.Customer
                              ? $"Khách sạn {c.HotelId.ToString().Substring(0, 4)}"
                              : $"Khách hàng {c.UserId.ToString().Substring(0, 4)}",
                AvatarUrl = request.CurrentUserRole == SenderType.Customer
                              ? "https://ui-avatars.com/api/?name=Hotel&background=random"
                              : "https://ui-avatars.com/api/?name=Customer&background=random"
            }).ToList();

            var pagedData = PagedResult<ConversationListDto?>.Create(
                items: dtos,
                totalCount: totalCount,
                pageNumber: request.PageIndex,
                pageSize: request.PageSize
            );

            return Result.Success(pagedData);
        }
    }
}
