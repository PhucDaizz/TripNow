using Domain.Common.Response;
using MediatR;

namespace RecommendationService.Application.Features.RAG.Queries.GetHotelChatContext
{
    public record GetHotelChatContextQuery(Guid HotelId, string UserMessage, int Limit = 3) : IRequest<Result<List<string>>>;
}
