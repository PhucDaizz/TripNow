using Domain.Common.Response;
using MediatR;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Domain.Repositories;

namespace RecommendationService.Application.Features.Recommendation.Queries
{
    public class GetPersonalizedRecommendationsQueryHandler : IRequestHandler<GetPersonalizedRecommendationsQuery, Result<IEnumerable<Guid>>>
    {
        private readonly IUserViewedHotelRepository _viewedHotelRepository;
        private readonly IQdrantService _qdrantService;

        public GetPersonalizedRecommendationsQueryHandler(
            IUserViewedHotelRepository viewedHotelRepository,
            IQdrantService qdrantService)
        {
            _viewedHotelRepository = viewedHotelRepository;
            _qdrantService = qdrantService;
        }

        public async Task<Result<IEnumerable<Guid>>> Handle(GetPersonalizedRecommendationsQuery request, CancellationToken cancellationToken)
        {
            var recentViews = await _viewedHotelRepository.GetByUserIdAsync(request.UserId, limit: 5);
            var viewedHotelIds = recentViews.Select(x => x.HotelId).ToList();

            if (!viewedHotelIds.Any())
            {
                return Result.Success<IEnumerable<Guid>>(new List<Guid>());
            }

            var recommendations = await _qdrantService.RecommendAsync(
                collectionName: "Hotels",
                positiveHotelIds: viewedHotelIds,
                limit: (ulong)request.Limit
            );

            var recommendedHotelIds = recommendations.Select(x => x.HotelId).ToList();

            return Result.Success<IEnumerable<Guid>>(recommendedHotelIds);
        }
    }
}
