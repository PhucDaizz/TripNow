using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.Comment.Commands.HideComment
{
    public class HideCommentCommand : IRequest<Result<bool>>
    {
        public Guid CommentId { get; set; }
        public string Reason { get; set; } // Lý do ẩn (VD: "Vi phạm tiêu chuẩn cộng đồng")
    }
}
