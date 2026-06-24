using Domain.Common.Response;
using MediatR;
using RecommendationService.Application.Common.Interfaces;

namespace RecommendationService.Application.Features.RAG.Queries.GetHotelChatContext
{
    public class GetHotelChatContextQueryHandler : IRequestHandler<GetHotelChatContextQuery, Result<List<string>>>
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IQdrantService _qdrantService;

        public GetHotelChatContextQueryHandler(
            IEmbeddingService embeddingService,
            IQdrantService qdrantService)
        {
            _embeddingService = embeddingService;
            _qdrantService = qdrantService;
        }

        public async Task<Result<List<string>>> Handle(GetHotelChatContextQuery request, CancellationToken cancellationToken)
        {
            var queryVector = await _embeddingService.GenerateEmbeddingAsync(request.UserMessage, cancellationToken);

            var contextChunks = await _qdrantService.SearchHotelDocumentsAsync(
                collectionName: "HotelKnowledgeBase",
                queryVector: queryVector,
                hotelId: request.HotelId,
                limit: (ulong)request.Limit
            );

            return Result.Success(contextChunks.ToList());
        }
    }
}
