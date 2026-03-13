using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.Comment.Commands.EditComment
{
    public class EditCommentCommandHandler : IRequestHandler<EditCommentCommand, Result<bool>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorIdentityService _authorIdentityService;

        public EditCommentCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IAuthorIdentityService authorIdentityService)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _authorIdentityService = authorIdentityService;
        }
        public async Task<Result<bool>> Handle(EditCommentCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = Guid.Parse(_currentUserService.UserId);
            var comment = await _unitOfWork.commentRepository.GetByIdAsync(request.CommentId, cancellationToken);

            if (comment == null || comment.IsDeleted)
                return Result.Failure<bool>(new Error("NOT.FOUND", "Comment does not exist."));

            bool hasPermission = false;

            if (comment.AuthorType == AuthorType.User && comment.AuthorId == currentUserId)
            {
                hasPermission = true;
            }
            else if (comment.AuthorType == AuthorType.Hotel)
            {
                var currentUserHotelContext = await _authorIdentityService.ResolveAuthorTypeAsync(comment.AuthorId, cancellationToken);

                if (currentUserHotelContext == AuthorType.Hotel)
                {
                    hasPermission = true; 
                }
            }

            if (!hasPermission)
            {
                return Result.Failure<bool>(new Error("NOT.PERMIT", "You do not have permission to edit this comment."));
            }

            try
            {
                comment.EditContent(request.Content);
                comment.ChangeUpdateBy(currentUserId);

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
