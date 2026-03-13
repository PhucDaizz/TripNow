using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.Comment.Commands.CreateComment
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorIdentityService _authorIdentityService;

        public CreateCommentCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService, 
            IAuthorIdentityService authorIdentityService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _authorIdentityService = authorIdentityService;
        }

        public async Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId!);

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

            var post = await _unitOfWork.postRepository.GetByIdAsync(request.PostId, cancellationToken);

            var authorType = await _authorIdentityService.ResolveAuthorTypeAsync(post.HotelId, cancellationToken);

            Guid authorId;
            if (authorType == AuthorType.Hotel && post.HotelId.HasValue)
            {
                authorId = post.HotelId.Value;
            }
            else
            {
                authorId = currentUserId;
            }

            var comment = new Domain.Entities.Comment(request.PostId, authorId, request.Content, authorType, request.ParentCommentId);
            comment.ChangeCreateBy(currentUserId);

            await _unitOfWork.commentRepository.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(comment.Id);
        }
    }
}
