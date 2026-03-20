using Domain.Common.Response;
using MediatR;
using RecommendationService.Application.Common.Interfaces;
using RecommendationService.Application.DTOs.UserSearchHistory;
using System.Collections.Generic;
using System.Linq;

namespace RecommendationService.Application.Features.UserSearchHistory.Queries.GetUserSearchHistory
{
    public class GetUserSearchHistoryQueryHandler : IRequestHandler<GetUserSearchHistoryQuery, Result<IEnumerable<UserSearchHistoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserSearchHistoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<UserSearchHistoryDto>>> Handle(GetUserSearchHistoryQuery request, CancellationToken cancellationToken)
        {
            var history = await _unitOfWork.UserSearchHistories.GetByUserIdAsync(request.UserId);

            var result = history.Select(h => new UserSearchHistoryDto
            {
                UserId = h.UserId,
                RawQuery = h.RawQuery,
                Destination = h.Destination,
                CheckInDate = h.CheckInDate,
                CheckOutDate = h.CheckOutDate,
                Adults = h.Adults,
                Children = h.Children,
                SearchedAt = h.SearchedAt
            });

            return Result.Success(result);
        }
    }
}
