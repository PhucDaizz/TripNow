using Api.Protos;
using Application.Features.User.Queries.IsUserExisting;
using Grpc.Core;
using MediatR;

namespace API.GrpcServices
{
    public class AuthGrpcEndpoint : AuthGrpc.AuthGrpcBase
    {
        private readonly IMediator _mediator;

        public AuthGrpcEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<IsUserExistingResponse> IsUserExisting(
            IsUserExistingRequest request,
            ServerCallContext context)
        {
            var query = new IsUserExistingQuery
            {
                UserId = Guid.Parse(request.UserId)
            };

            var result = await _mediator.Send(query);

            return new IsUserExistingResponse
            {
                IsExisting = result.Value 
            };
        }
    }
}
