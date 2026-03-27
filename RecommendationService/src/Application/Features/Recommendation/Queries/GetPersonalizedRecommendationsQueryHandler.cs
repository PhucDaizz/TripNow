using Domain.Common.Response;
using MediatR;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Application.DTOs.Recommendation;
using RecommendationService.Domain.Repositories;

namespace RecommendationService.Application.Features.Recommendation.Queries
{
    public class GetPersonalizedRecommendationsQueryHandler : IRequestHandler<GetPersonalizedRecommendationsQuery, Result<IEnumerable<Guid>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQdrantService _qdrantService;
        private readonly IEmbeddingService _embeddingService;

        public GetPersonalizedRecommendationsQueryHandler(
            IUnitOfWork unitOfWork,
            IQdrantService qdrantService, 
            IEmbeddingService embeddingService)
        {
            _unitOfWork = unitOfWork;
            _qdrantService = qdrantService;
            _embeddingService = embeddingService;
        }

        public async Task<Result<IEnumerable<Guid>>> Handle(GetPersonalizedRecommendationsQuery request, CancellationToken cancellationToken)
        {
            var recentViews = await _unitOfWork.UserViewedHotels.GetByUserIdAsync(request.UserId, limit: 5);
            var viewedHotelIds = recentViews.Select(x => x.HotelId).ToList();

            var latestSearchList = await _unitOfWork.UserSearchHistories.GetByUserIdAsync(request.UserId, 1);
            var latestSearch = latestSearchList.FirstOrDefault();
            string? targetDestination = latestSearch?.Destination;
            string? rawQuery = latestSearch?.RawQuery;

            // Kịch bản Blank Slate (0 view, 0 search)
            if (!viewedHotelIds.Any() && string.IsNullOrWhiteSpace(rawQuery) && string.IsNullOrWhiteSpace(targetDestination))
            {
                return Result.Success<IEnumerable<Guid>>(await _qdrantService.GetRandomHotelsAsync("Hotels", (ulong)request.Limit));
            }

            // 2. CHUẨN BỊ BỘ LỌC CHUNG (Thành phố)
            var viewedCities = viewedHotelIds.Any()
                ? await _qdrantService.GetCitiesByHotelIdsAsync("Hotels", viewedHotelIds)
                : new List<string>();

            if (!string.IsNullOrWhiteSpace(targetDestination) && !viewedCities.Contains(targetDestination))
            {
                viewedCities.Insert(0, targetDestination); // Nhét Destination vào bộ lọc Should
            }

            // 3. KHỞI TẠO 2 TASK CHẠY SONG SONG
            Task<IReadOnlyList<VectorSearchResult>> searchTask = Task.FromResult((IReadOnlyList<VectorSearchResult>)new List<VectorSearchResult>());
            Task<IReadOnlyList<VectorSearchResult>> recommendTask = Task.FromResult((IReadOnlyList<VectorSearchResult>)new List<VectorSearchResult>());

            if (!string.IsNullOrWhiteSpace(rawQuery))
            {
                var queryVector = await _embeddingService.GenerateEmbeddingAsync(rawQuery);

                searchTask = _qdrantService.SearchSimilarAsync("Hotels", queryVector, limit: (ulong)request.Limit);
            }

            if (viewedHotelIds.Any())
            {
                recommendTask = _qdrantService.RecommendAsync("Hotels", viewedHotelIds, viewedCities, limit: (ulong)request.Limit);
            }

            await Task.WhenAll(searchTask, recommendTask);

            var searchResults = searchTask.Result;
            var recommendResults = recommendTask.Result;

            var finalHotelIds = new List<Guid>();

            var intersectedIds = searchResults.Select(x => x.HotelId)
                .Intersect(recommendResults.Select(x => x.HotelId))
                .ToList();
            finalHotelIds.AddRange(intersectedIds);

            int maxLen = Math.Max(searchResults.Count, recommendResults.Count);
            for (int i = 0; i < maxLen; i++)
            {
                if (finalHotelIds.Count >= request.Limit) break;

                // Lấy từ Search (Ý định gõ chữ luôn rõ ràng hơn)
                if (i < searchResults.Count && !finalHotelIds.Contains(searchResults[i].HotelId))
                    finalHotelIds.Add(searchResults[i].HotelId);

                if (finalHotelIds.Count >= request.Limit) break;

                // Lấy từ Recommend (Bổ sung theo Gu)
                if (i < recommendResults.Count && !finalHotelIds.Contains(recommendResults[i].HotelId))
                    finalHotelIds.Add(recommendResults[i].HotelId);
            }

            // Fallback: Random nếu thiếu (Data mỏng)
            if (finalHotelIds.Count < request.Limit)
            {
                var fallbackHotels = await _qdrantService.GetRandomHotelsAsync("Hotels", (ulong)(request.Limit - finalHotelIds.Count));
                finalHotelIds.AddRange(fallbackHotels.Where(id => !finalHotelIds.Contains(id)));
            }

            return Result.Success<IEnumerable<Guid>>(finalHotelIds.Take(request.Limit));
        }
    }
}
