using Domain.Common.Response;
using MediatR;

namespace RecommendationService.Application.Features.Recommendation.Queries
{
    public record GetPersonalizedRecommendationsQuery(Guid UserId, int Limit = 10) : IRequest<Result<IEnumerable<Guid>>>;
}
