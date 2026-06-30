using Grpc.Core;
using MediatR;
using RecommendationService.Api.Protos;
using RecommendationService.Application.Features.RAG.Queries.GetHotelChatContext;

namespace RecommendationService.API.GrpcServices
{
    public class RagGrpcEndpoint : RagGrpc.RagGrpcBase
    {
        private readonly IMediator _mediator;

        public RagGrpcEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<GetHotelChatContextResponse> GetHotelChatContext(
            GetHotelChatContextRequest request,
            ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "The question cannot be left blank."));
            }

            var query = new GetHotelChatContextQuery(
                Guid.Parse(request.HotelId),
                request.Message,
                request.Limit);

            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, result.Error.Message));
            }

            var response = new GetHotelChatContextResponse();

            if (result.Value != null)
            {
                response.Contexts.AddRange(result.Value);
            }

            return response;
        }
    }
}
