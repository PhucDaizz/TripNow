using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;

namespace SocialService.Application.Features.Comment.Commands.HideComment
{
    public class HideCommentCommandHandler : IRequestHandler<HideCommentCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public HideCommentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(HideCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.commentRepository.GetByIdAsync(request.CommentId);

            if (comment == null)
            {
                return Result.Failure<bool>(new Error("NOT.FOUND", "Comment is not existing!"));
            }

            comment.HideByModerator(request.Reason);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
