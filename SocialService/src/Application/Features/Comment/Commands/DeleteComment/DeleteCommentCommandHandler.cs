using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;

namespace SocialService.Application.Features.Comment.Commands.DeleteComment
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result<bool>>
    {
        private readonly ICurrentUserService  _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCommentCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);
            var comment = await _unitOfWork.commentRepository.GetByIdAsync(request.CommentId);

            if (comment == null || comment.IsDeleted)
                return Result.Failure<bool>(new Error("NOT.EXISTING", "The comment does not exist."));

            if (comment.UserId != userId)
                return Result.Failure<bool>(new Error("NOT.PERMIT","You do not have the right to delete this comment."));
            
            comment.Delete(userId);

            await _unitOfWork.commentRepository.UpdateAsync(comment, cancellationToken); 
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
