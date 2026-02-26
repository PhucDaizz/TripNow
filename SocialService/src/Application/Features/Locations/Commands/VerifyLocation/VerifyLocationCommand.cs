using Domain.Common.Response;
using MediatR;

namespace SocialService.Application.Features.Locations.Commands.VerifyLocation
{
    public class VerifyLocationCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
    }
}
