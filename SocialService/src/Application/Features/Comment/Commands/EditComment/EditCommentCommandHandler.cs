using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;

namespace SocialService.Application.Features.Comment.Commands.EditComment
{
    public class EditCommentCommandHandler : IRequestHandler<EditCommentCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public EditCommentCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<bool>> Handle(EditCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);
            var comment = await _unitOfWork.commentRepository.GetByIdAsync(request.CommentId, cancellationToken);

            if (comment == null || comment.IsDeleted)
                return Result.Failure<bool>(new Error("NOT.FOUND", "Comment does not exist."));

            if (comment.UserId != userId)
                return Result.Failure<bool>(new Error("NOT.PERMIT", "You do not have permission to edit this comment."));

            try
            {
                comment.EditContent(request.Content, userId);

                await _unitOfWork.commentRepository.UpdateAsync(comment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>(new Error("PROGRAM.ERROR",ex.Message));
            }
        }
    }
}
