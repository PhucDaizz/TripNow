using Domain.Common.Response;
using MediatR;
using RecommendationService.Application.Common.Interfaces;

namespace RecommendationService.Application.Features.Recommendation.Queries
{
    public class GetSimilarHotelsQueryHandler : IRequestHandler<GetSimilarHotelsQuery, Result<IEnumerable<Guid>>>
    {
        private const string CollectionName = "Hotels";

        private readonly IQdrantService _qdrantService;

        public GetSimilarHotelsQueryHandler(IQdrantService qdrantService)
        {
            _qdrantService = qdrantService;
        }

        public async Task<Result<IEnumerable<Guid>>> Handle(GetSimilarHotelsQuery request, CancellationToken cancellationToken)
        {
            // Dùng Qdrant Recommend API: lấy vector của hotel đang xem làm "positive example",
            // Qdrant sẽ tìm các hotel có vector tương tự nhất nhưng không phải chính hotel đó.
            var results = await _qdrantService.RecommendAsync(
                collectionName  : CollectionName,
                positiveHotelIds: [request.HotelId],
                limit           : (ulong)request.Limit);

            var similarHotelIds = results
                .Where(r => r.HotelId != request.HotelId)
                .Select(r => r.HotelId);

            return Result.Success<IEnumerable<Guid>>(similarHotelIds);
        }
    }
}
