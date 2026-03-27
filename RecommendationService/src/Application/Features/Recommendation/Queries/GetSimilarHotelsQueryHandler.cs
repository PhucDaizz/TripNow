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
            var cities = await _qdrantService.GetCitiesByHotelIdsAsync("Hotels", new[] { request.HotelId });
            var targetCity = cities.FirstOrDefault();

            if (string.IsNullOrEmpty(targetCity))
            {
                return Result.Failure<IEnumerable<Guid>>(new Error("Hotel.NotFound", "Không tìm thấy dữ liệu Khách sạn này trên hệ thống AI."));
            }

            var recommendations = await _qdrantService.GetSimilarHotelsAdvancedAsync(
                collectionName: "Hotels",
                currentHotelId: request.HotelId,
                targetCity: targetCity,
                totalLimit: (ulong)request.Limit
            );

            var recommendedHotelIds = recommendations.Select(x => x.HotelId).ToList();

            return Result.Success<IEnumerable<Guid>>(recommendedHotelIds);
        }
    }
}
