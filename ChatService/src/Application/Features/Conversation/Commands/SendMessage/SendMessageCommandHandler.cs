using ChatService.Application.Common.Interfaces;
using ChatService.Application.DTOs.Message;
using ChatService.Application.Interface;
using ChatService.Domain.Entities;
using ChatService.Domain.Enum;
using Domain.Common.Response;
using MediatR;

namespace ChatService.Application.Features.Conversation.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<MessageDto>>
    {
        private readonly IChatNotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public SendMessageCommandHandler(
            IChatNotificationService notificationService, 
            IUnitOfWork unitOfWork)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var conversation = await _unitOfWork.Conversation.GetByIdAsync(request.ConversationId, cancellationToken);
            if (conversation == null)
            {
                return Result.Failure<MessageDto>(new Error("NOT.FOUND","Not found for this conversation"));
            }

            var newMessage = Messages.CreateMessage(
                conversationId: request.ConversationId,
                senderId: request.CurrentUserId,
                senderType: request.CurrentUserRole,
                content: request.Content,
                messageType: request.MessageType
            );

            conversation.AddMessage(newMessage);
            conversation.UpdateLastMessage(request.Content, request.CurrentUserRole);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var messageDto = new MessageDto
            {
                Id = newMessage.Id,
                ConversationId = newMessage.ConversationId,
                SenderId = newMessage.SenderId,
                SenderType = newMessage.SenderType,
                Content = newMessage.Content,
                MessageType = newMessage.MessageType,
                IsRead = newMessage.IsRead,
                CreatedAt = DateTime.UtcNow,
                SenderName = request.CurrentUserName
            };

            Guid receiverId = request.CurrentUserRole == SenderType.Hotel
                  ? conversation.UserId 
                  : conversation.HotelId;

            await _notificationService.SendMessageToConversationAsync(request.ConversationId, receiverId, messageDto);

            if (request.CurrentUserRole == SenderType.Customer)
            {
                await _notificationService.SendMessageToHotelStaffAsync(conversation.HotelId, messageDto);
            }

            return Result.Success(messageDto);
        }
    }
}
