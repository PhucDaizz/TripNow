using Domain.Common.Response;
using MediatR;
using RecommendationService.Application.DTOs.UserSearchHistory;
using System.Collections.Generic;

namespace RecommendationService.Application.Features.UserSearchHistory.Queries.GetUserSearchHistory
{
    public class GetUserSearchHistoryQuery : IRequest<Result<IEnumerable<UserSearchHistoryDto>>>
    {
        public Guid UserId { get; set; }
    }
}
