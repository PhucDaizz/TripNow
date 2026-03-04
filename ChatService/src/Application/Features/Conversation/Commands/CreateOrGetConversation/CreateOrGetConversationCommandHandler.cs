using ChatService.Application.Common.Interfaces;
using ChatService.Domain.Entities;
using Domain.Common.Response;
using MediatR;

namespace ChatService.Application.Features.Conversation.Commands.CreateOrGetConversation
{
    public class CreateOrGetConversationCommandHandler : IRequestHandler<CreateOrGetConversationCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrGetConversationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateOrGetConversationCommand request, CancellationToken cancellationToken)
        {
            var existingConversation = await _unitOfWork.Conversation
                .GetByUserIdAndHotelIdAsync(request.CurrentUserId, request.HotelId, cancellationToken);

            if (existingConversation != null)
            {
                return Result.Success(existingConversation.Id);
            }

            var newConversation = Conversations.Create(request.CurrentUserId, request.HotelId);

            await _unitOfWork.Conversation.AddAsync(newConversation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(newConversation.Id);
        }
    }
}
