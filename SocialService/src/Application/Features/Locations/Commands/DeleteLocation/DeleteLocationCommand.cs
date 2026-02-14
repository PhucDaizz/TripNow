using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.Locations.Commands.DeleteLocation
{
    public class DeleteLocationCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
    }
}
