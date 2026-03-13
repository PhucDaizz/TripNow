using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;
using SocialService.Domain.Common;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.Comment.Commands.DeleteComment
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Result<bool>>
    {
        private readonly ICurrentUserService  _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorIdentityService _authorIdentityService;

        public DeleteCommentCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IAuthorIdentityService authorIdentityService)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _authorIdentityService = authorIdentityService;
        }

        public async Task<Result<bool>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId);
            var comment = await _unitOfWork.commentRepository.GetByIdAsync(request.CommentId);

            if (comment == null || comment.IsDeleted)
                return Result.Failure<bool>(new Error("NOT.EXISTING", "The comment does not exist."));

            var post = await _unitOfWork.postRepository.GetByIdAsync(comment.PostId, cancellationToken);
            bool isAdmin = _currentUserService.Role?.Contains(AppRoles.SysAdmin) ?? false;

            bool hasPermission = false;

            if (isAdmin)
            {
                hasPermission = true;
            }
            else if (comment.AuthorType == AuthorType.User && comment.AuthorId == currentUserId)
            {
                hasPermission = true; 
            }
            else if (comment.AuthorType == AuthorType.Hotel)
            {
                var currentUserHotelContext = await _authorIdentityService.ResolveAuthorTypeAsync(comment.AuthorId, cancellationToken);
                if (currentUserHotelContext == AuthorType.Hotel) hasPermission = true;
            }

            if (!hasPermission && post != null)
            {
                if (post.AuthorType == AuthorType.User && post.AuthorId == currentUserId)
                {
                    hasPermission = true; 
                }
                else if (post.AuthorType == AuthorType.Hotel)
                {
                    var currentUserHotelContext = await _authorIdentityService.ResolveAuthorTypeAsync(post.AuthorId, cancellationToken);
                    if (currentUserHotelContext == AuthorType.Hotel) hasPermission = true;
                }
            }

            if (!hasPermission)
            {
                return Result.Failure<bool>(new Error("NOT.PERMIT", "You do not have the right to delete this comment."));
            }

            comment.Delete();
            comment.ChangeUpdateBy(currentUserId);

            await _unitOfWork.commentRepository.UpdateAsync(comment, cancellationToken); 
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
