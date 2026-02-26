using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;

namespace SocialService.Application.Features.Comment.Commands.CreateComment
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateCommentCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUserService.UserId);

            var postExists = await _unitOfWork.postRepository.IsPostExisting(request.PostId, cancellationToken);
            if (!postExists)
            {
                return Result.Failure<Guid>(new Error("NOT.FOUND","The post does not exist or has been deleted."));
            }

            if (request.ParentCommentId.HasValue)
            {
                var parentComment = await _unitOfWork.commentRepository.GetByIdAsync(request.ParentCommentId.Value);

                if (parentComment == null || parentComment.IsDeleted)
                    return Result.Failure<Guid>(new Error("COMMENT.NOTFOUND", "The original comment does not exist."));

                if (parentComment.PostId != request.PostId)
                {
                    return Result.Failure<Guid>(new Error("COMMENT.INVALID", "The reply does not belong to the requested post."));
                }

                if (parentComment.ParentCommentId.HasValue)
                {
                    request.ParentCommentId = parentComment.ParentCommentId; 
                }
            }

            var comment = new Domain.Entities.Comment(request.PostId, userId, request.Content, request.ParentCommentId);

            await _unitOfWork.commentRepository.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(comment.Id);
        }
    }
}
