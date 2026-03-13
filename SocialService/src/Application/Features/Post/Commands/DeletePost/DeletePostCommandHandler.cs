using Domain.Common.Response;
using MediatR;
using SocialService.Application.Common.Interfaces;
using SocialService.Application.Contracts;
using SocialService.Domain.Common;
using SocialService.Domain.Enum;

namespace SocialService.Application.Features.Post.Commands.DeletePost
{
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorIdentityService _authorIdentityService;

        public DeletePostCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IAuthorIdentityService authorIdentityService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _authorIdentityService = authorIdentityService;
        }

        public async Task<Result<bool>> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            //var userId = Guid.Parse(_currentUserService.UserId);

            var currentUserId = Guid.Parse(_currentUserService.UserId!);
            var post = await _unitOfWork.postRepository.GetByIdAsync(request.PostId);

            if (post == null || post.IsDeleted)
                return Result.Failure<bool>(new Error("NOT_FOUND", "Bài viết không tồn tại."));

            bool isAdmin = _currentUserService.Role?.Contains(AppRoles.SysAdmin) ?? false;

            var currentUserHotelContext = await _authorIdentityService.ResolveAuthorTypeAsync(post.HotelId, cancellationToken);

            bool hasPermission = false;

            if (isAdmin)
            {
                hasPermission = true;
            }
            else if (post.AuthorType == AuthorType.User && post.AuthorId == currentUserId)
            {
                hasPermission = true; 
            }
            else if (currentUserHotelContext == AuthorType.Hotel)
            {
                hasPermission = true;
            }

            if (!hasPermission)
            {
                return Result.Failure<bool>(new Error("FORBIDDEN", "Bạn không có quyền xóa bài viết này."));
            }

            post.Delete();

            await _unitOfWork.postRepository.UpdateAsync(post, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
