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

            if (!conversations.Any())
            {
                return Result.Success(PagedResult<ConversationListDto?>.Create(new List<ConversationListDto?>(), 0, request.PageIndex, request.PageSize));
            }

            var targetProfileIds = request.CurrentUserRole == SenderType.Customer
                ? conversations.Select(c => c.HotelId).Distinct().ToList() 
                : conversations.Select(c => c.UserId).Distinct().ToList(); 

            var profiles = await _unitOfWork.ChatProfile.GetByIdsAsync(targetProfileIds, cancellationToken);

            var profileDict = profiles.ToDictionary(p => p.Id);

            var dtos = conversations.Select(c =>
            {
                var targetId = request.CurrentUserRole == SenderType.Customer ? c.HotelId : c.UserId;

                profileDict.TryGetValue(targetId, out var targetProfile);

                return new ConversationListDto
                {
                    Id = c.Id,
                    HotelId = c.HotelId,
                    UserId = c.UserId,
                    LastMessage = c.LastMessage ?? string.Empty,
                    UpdatedAt = c.UpdatedAt ?? c.CreatedAt,
                    UnreadCount = request.CurrentUserRole == SenderType.Customer
                                  ? c.CustomerUnreadCount
                                  : c.HotelUnreadCount,

                    DisplayName = targetProfile?.FullName ?? "Người dùng ẩn danh",
                    AvatarUrl = string.IsNullOrEmpty(targetProfile?.AvatarUrl)
                                ? $"https://ui-avatars.com/api/?name={(request.CurrentUserRole == SenderType.Customer ? "Hotel" : "Customer")}&background=random"
                                : targetProfile.AvatarUrl
                };
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
