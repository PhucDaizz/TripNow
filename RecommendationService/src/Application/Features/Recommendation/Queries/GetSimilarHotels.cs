using Domain.Common.Response;
using MediatR;

namespace RecommendationService.Application.Features.Recommendation.Queries
{
    /// <summary>
    /// Trả về danh sách HotelId của các khách sạn tương tự với khách sạn đang xem.
    /// Dùng khi người dùng vào trang chi tiết 1 khách sạn.
    /// </summary>
    public record GetSimilarHotelsQuery(Guid HotelId, int Limit = 10) : IRequest<Result<IEnumerable<Guid>>>;
}
