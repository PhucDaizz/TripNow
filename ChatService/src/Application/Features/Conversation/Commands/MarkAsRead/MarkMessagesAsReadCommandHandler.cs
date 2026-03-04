using ChatService.Application.Common.Interfaces;
using ChatService.Application.Interface;
using Domain.Common.Response;
using MediatR;

namespace ChatService.Application.Features.Conversation.Commands.MarkAsRead
{
    public class MarkMessagesAsReadCommandHandler : IRequestHandler<MarkMessagesAsReadCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatNotificationService _notificationService;

        public MarkMessagesAsReadCommandHandler(IUnitOfWork unitOfWork, IChatNotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<Result<bool>> Handle(MarkMessagesAsReadCommand request, CancellationToken cancellationToken)
        {
            var conversation = await _unitOfWork.Conversation.GetByIdAsync(request.ConversationId, cancellationToken);
            if (conversation == null) return Result.Failure<bool>(new Error("NOT.FOUND", "Không tìm thấy phòng chat"));

            conversation.ResetUnreadCount(request.CurrentUserRole);

            var unreadMessages = await _unitOfWork.Conversation.GetUnreadMessage(request.CurrentUserId, request.ConversationId, cancellationToken);
            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                {
                    msg.MarkAsRead(); 
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _notificationService.SendReadReceiptAsync(request.ConversationId, request.CurrentUserId);

            return Result.Success(true);
        }
    }
}
